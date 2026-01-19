using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GlobalSettings;
using Utils;
using Workspace.Facilities;

namespace Workspace
{
    [Serializable]
    public class WorkspaceManager : Singleton<WorkspaceManager>
    {
        private List<Facility> facilities;
        private int edgeLength;
        
        public Facility GetFacility(int x, int y)
        {
            x = x % edgeLength; y = y % edgeLength;
            return facilities[x + y*edgeLength];
        }
        public bool TrySetFacility(int x, int y, Facility facility)
        {
            var index = x + y * edgeLength;
            var oldFacility = facilities[index];
            if (facility.CanBuildOn(oldFacility.Type)) facilities[index] = facility; else return false;
            return true;
        }
        public bool Resize(int newEdgeLength)
        {
            if (newEdgeLength < 1 || newEdgeLength > GlobalConsts.MaxWorkspaceEdgeLength) return false;
            edgeLength = newEdgeLength;
            facilities = new List<Facility> { Capacity = newEdgeLength * newEdgeLength };
            for (var i = newEdgeLength * newEdgeLength - 1; i >= 0 ; i--) 
                facilities[i] = FacilityFactory.CreateEmpty(i % newEdgeLength, i / newEdgeLength);
            return true;
        }
        
        public WorkspaceManager()
        {
            edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
            Resize(edgeLength);
        }
    }
}