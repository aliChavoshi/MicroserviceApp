namespace Ordering.Core.Entities.Base
{
    public interface IEntityBase<out T>
    {
        T Id { get; }
    }
}