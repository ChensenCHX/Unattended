using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using Utils;
using Workspace.Facilities;

namespace Workspace
{
    [Serializable]
    public class WorkspaceManager : SingletonMono<WorkspaceManager>
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
            var totalCount = newEdgeLength * newEdgeLength;
            facilities = new List<Facility>(totalCount);
            for (var i = 0; i < totalCount ; i++) 
                facilities.Add(FacilityFactory.CreateEmpty(i % newEdgeLength, i / newEdgeLength));
            return true;
        }

        private void Start()
        {
            ResourceManager<GameObject>.AddSearchPath("Facilities/Prefabs");
            ResourceManager<GameObject>.LoadAll();
            edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
            Resize(edgeLength);
        }
    }
}