using UnityEngine;
using Utils;
using Workspace.Facilities.Impl;

namespace Workspace.Facilities
{
    public static class FacilityFactory
    {
        public static Facility CreateEmpty(int x, int y)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var obj = GameObject.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityEmpty"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityEmpty>();
            facility.Init(x, y);
            return facility;
        }
        
    }
}