using System;
using System.Threading;
using System.Timers;
using Memory;
using PropertyChanged.SourceGenerator;
using ShipbreakerCompanion.Client.Commands;
using ShipbreakerCompanion.Client.Models;
using Timer = System.Timers.Timer;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// ViewModel for the "current salvage" view.
    /// </summary>
    public partial class CurrentSalvageViewModel
    {
        /// <summary>
        /// Current tracking state.
        /// </summary>
        [Notify] private GameTrackingState _trackingState;

        /// <summary>
        /// Total salvageable value of the current ship.
        /// </summary>
        [Notify] private float _totalSalvageableValue;

        /// <summary>
        /// Current salvaged value.
        /// </summary>
        [Notify] private float _salvagedValue;

        /// <summary>
        /// Current destroyed value.
        /// </summary>
        [Notify] private float _destroyedValue;

        /// <summary>
        /// Salvage progress indicator. Has to be the same as the in-game equivalent.
        /// </summary>
        [Notify] private string _salvageProgress;

        /// <summary>
        /// Destroy progress indicator.
        /// </summary>
        [Notify] private string _destroyProgress;

        /// <summary>
        /// Current state of the salvaging objective.
        /// </summary>
        [Notify] private SalvageObjectiveState _objectiveState;

        /// <summary>
        /// Percentage of salvage to reach to complete the objective.
        /// </summary>
        [Notify] private float _salvagingObjectivePercentage = 95; //todo: Make this configurable

        /// <summary>
        /// Error message related to tracking.
        /// </summary>
        [Notify] private string _stateError;

        /// <summary>
        /// Gets a boolean value indicating if the tracking is currently fully operational.
        /// </summary>
        public bool IsFullyTracking => TrackingState == GameTrackingState.Tracking;

        /// <summary>
        /// Gets a boolean value indicating if the tracking is in the stopped state.
        /// </summary>
        public bool IsTrackingStopped => TrackingState == GameTrackingState.Stopped;

        /// <summary>
        /// Gets a boolean value indicating if the tracking is in the Attached state.
        /// </summary>
        public bool IsTrackingSearching => TrackingState == GameTrackingState.Attached;

        /// <summary>
        /// Gets the command that toggles tracking.
        /// </summary>
        public RelayCommand<CurrentSalvageViewModel> ToggleTrackingCommand { get; }

        private readonly Timer _refreshTimer;
        private Mem _mem;
        private long? _salvageProgressAddress;

        /// <summary>
        /// Instanciates the view model.
        /// </summary>
        public CurrentSalvageViewModel()
        {
            _mem = new Mem();
            _refreshTimer = new Timer(200);
            _refreshTimer.AutoReset = false;
            _refreshTimer.Elapsed += OnRefreshTick;

            ToggleTrackingCommand = new RelayCommand<CurrentSalvageViewModel>(ToggleTracking);
        }

        /// <summary>
        /// Starts or stops tracking depending on the current state.
        /// </summary>
        /// <param name="vm">Source ViewModel.</param>
        private static void ToggleTracking(CurrentSalvageViewModel vm)
        {
            switch (vm.TrackingState)
            {
                case GameTrackingState.Stopped:
                    vm.StartTracking();
                    break;
                case GameTrackingState.Attached:
                case GameTrackingState.Tracking:
                default:
                    vm.StopTracking();
                    break;
            }
        }

        /// <summary>
        /// Callback. Called when the refresh timer ticks.
        /// Performs tracking operations depending on the current tracking state.
        /// </summary>
        private void OnRefreshTick(object sender, ElapsedEventArgs e)
        {
            switch (TrackingState)
            {
                case GameTrackingState.Attached:
                    TryFindTrackedValues();
                    _refreshTimer.Start();
                    break;
                case GameTrackingState.Tracking:
                    TryReadTrackedValues();
                    _refreshTimer.Start();
                    break;
                case GameTrackingState.Stopped:
                default:
                    return;
            }
        }

        /// <summary>
        /// Turns the tracking on.
        /// </summary>
        public void StartTracking()
        {
            if (!_mem.OpenProcess("Shipbreaker", out var openError))
            {
                StateError = openError;
                return;
            }

            TrackingState = GameTrackingState.Attached;
            _refreshTimer.Start();
        }

        /// <summary>
        /// Turns the tracking off.
        /// </summary>
        public void StopTracking()
        {
            TrackingState = GameTrackingState.Stopped;
            _refreshTimer.Stop();
            _mem.CloseProcess();

            // Memory.dll does not support opening a process again after closing it.
            // We have to make a new instance.
            _mem = new Mem();
        }

        /// <summary>
        /// Attempts to locate the area in the game memory where the values we want to track can be found.
        /// </summary>
        private void TryFindTrackedValues()
        {
            try
            {
                // Perform an array-of-byte scan to find the salvage progress state struct.
                // This relies on the SalvageIndicatorController instance having the right enum/bool values.
                // Found with the help of Cheat Engine with the Mono plugin.
                var aobResults = _mem.AoBScan("01 00 00 00 02 00 00 00 01 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 01 01 01", true, false).Result;

                // This will probably return several results because it's not a great signature (best I could find after a whole day).
                // We'll try to read from each result and see if the numbers look right for each.
                foreach (var result in aobResults)
                {
                    // From the aob signature, the struct values are found at +40.
                    long tentativeProgressAddress = result + 40;
                    var tentativeState = ReadSalvageState(tentativeProgressAddress);
                    if (IsStateCoherent(tentativeState))
                    {
                        // The values look right. Use this address.
                        _salvageProgressAddress = tentativeProgressAddress;
                        TrackingState = GameTrackingState.Tracking;
                        SetCurrentSalvageState(tentativeState);
                        return;
                    }
                }

                // We didn't find the right address. Sleep for a while so that we don't spam AoB scans (they're CPU-intensive).
                Thread.Sleep(5000);
            }
            catch
            {
                // If the readings fail here, stop tracking.
                StopTracking();
            }
        }

        /// <summary>
        /// Reads the memory at the given address to find the salvaging state.
        /// </summary>
        /// <param name="address">Start address of the salvaging state struct.</param>
        private TrackedSalvageState ReadSalvageState(long address)
        {
            // The struct contains the 3 same floats that we have in our struct, in the same order.
            float totalSalvageableValue = _mem.ReadFloat((address).ToString("X"));
            float salvagedValue = _mem.ReadFloat((address + 4).ToString("X"));
            float destroyedValue = _mem.ReadFloat((address + 8).ToString("X"));

            return new TrackedSalvageState(totalSalvageableValue, salvagedValue, destroyedValue);
        }

        /// <summary>
        /// Updates the ViewModel to match the given salvaging state.
        /// </summary>
        /// <param name="state">Target state.</param>
        private void SetCurrentSalvageState(TrackedSalvageState state)
        {
            TotalSalvageableValue = state.TotalSalvageableValue;
            SalvagedValue = state.SalvagedValue;
            DestroyedValue = state.DestroyedValue;

            SalvageProgress = GetValueProgressPercentageAsDisplayedInGame(SalvagedValue, TotalSalvageableValue);
            DestroyProgress = GetValueProgressPercentageAsDisplayedInGame(DestroyedValue, TotalSalvageableValue);
            ObjectiveState = ComputeObjectiveState(state);
        }

        /// <summary>
        /// Given a salvaging state, computes the state of the salvaging objective.
        /// </summary>
        /// <param name="state">Target state.</param>
        private SalvageObjectiveState ComputeObjectiveState(TrackedSalvageState state)
        {
            float salvageRatio = state.TotalSalvageableValue == 0 ? 0 : state.SalvagedValue / state.TotalSalvageableValue;
            float damageRatio = state.TotalSalvageableValue == 0 ? 0 : state.DestroyedValue / state.TotalSalvageableValue;

            float objectiveRatio = SalvagingObjectivePercentage / 100;
            float allowedDamageRatio = (100 - SalvagingObjectivePercentage) / 100;
            if (damageRatio > allowedDamageRatio)
                return SalvageObjectiveState.Failed;
            return salvageRatio >= objectiveRatio ? SalvageObjectiveState.Reached : SalvageObjectiveState.InProgress;
        }

        /// <summary>
        /// Formats a progress percentage using the same rules as the game.
        /// </summary>
        /// <param name="value">Value to be divided by the total to compute progress.</param>
        /// <param name="totalValue">Total value. The <paramref name="value"/> will be divided by this to compute progress.</param>
        private string GetValueProgressPercentageAsDisplayedInGame(float value, float totalValue)
        {
            if (totalValue == 0)
                return "0%";

            float ratio = value / totalValue;
            return Math.Round(ratio * 100, 1).ToString("F1") + "%";
        }

        /// <summary>
        /// Attempts to read the tracked values and load them into the ViewModel.
        /// </summary>
        private void TryReadTrackedValues()
        {
            long? structAddress = _salvageProgressAddress;
            if (structAddress == null)
                return;

            try
            {
                var tentativeState = ReadSalvageState(structAddress.Value);
                if (IsStateCoherent(tentativeState))
                {
                    SetCurrentSalvageState(tentativeState);
                }
                else
                {
                    // State is incoherent. Go back to looking for the right values.
                    TrackingState = GameTrackingState.Attached;
                }
            }
            catch
            {
                // Roll back the state to Attached in case of an error.
                // This will cause the next tick to look for fall back to
                // searching for the values to track.
                TrackingState = GameTrackingState.Attached;
            }
        }

        /// <summary>
        /// Checks if the values of the given state seem to be correct, or if they are bogus.
        /// </summary>
        /// <param name="state">State to test.</param>
        private bool IsStateCoherent(TrackedSalvageState state)
        {
            // Start by checking if the total salvageable value is something that can happen in the game.
            return state.TotalSalvageableValue is >= 1000000 and <= 100000000
                   // Salvaged value and destroyed value should both be positive and below the total.
                   // We allow some leeway for rounding errors because of floats.
                   && state.SalvagedValue >= -1 && state.SalvagedValue <= state.TotalSalvageableValue + 10
                   && state.DestroyedValue >= -1 && state.DestroyedValue <= state.TotalSalvageableValue + 10
                   // Salvage + destroyed cannot exceed total salvageable. Also allow leeway here.
                   && state.TotalSalvageableValue >= state.SalvagedValue + state.DestroyedValue - 10;
        }
    }
}
