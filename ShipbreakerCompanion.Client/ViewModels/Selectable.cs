using PropertyChanged.SourceGenerator;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// ViewModel for selectable items to be used in collections.
    /// Allows for easier binding synchronization with ItemsControls.
    /// </summary>
    /// <typeparam name="T">Type of the underlying data.</typeparam>
    public partial class Selectable<T> where T: class
    {
        /// <summary>
        /// Underlying data.
        /// </summary>
        [Notify] private T _value;

        /// <summary>
        /// Defines whether the item is selected or not.
        /// </summary>
        [Notify] private bool _isSelected;
    }
}
