using MoonSharp.Interpreter;
using Items;

namespace Workspace.Facilities
{
    public abstract class Facility
    {
        # region 属性
        public abstract FacilityType Type { get; }
        public abstract double Progress { get; }
        public abstract int X { get; }
        public abstract int Y { get; }
        # endregion
        
        # region 管理器方法
        public abstract bool CanBuildOn(FacilityType type);
        # endregion

        # region 玩家方法
        public abstract DynValue GetUniqueState();
        public abstract DynValue InteractWith(DynValue argTuple);
        public abstract DynValue TryAddItem(ItemType item, int count);
        public abstract DynValue CanHarvest();
        public abstract DynValue Harvest();
        # endregion
    }
}
