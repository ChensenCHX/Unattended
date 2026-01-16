using Workspace.Facilities.Impl;

namespace Workspace.Facilities
{
    public static class FacilityFactory
    {
        public static Facility CreateEmpty(int x, int y) => new FacilityEmpty(x, y);
        
    }
}