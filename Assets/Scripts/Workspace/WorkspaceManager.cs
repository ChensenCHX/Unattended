using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using Utils;
using Workspace.Facilities;

namespace Workspace
{
    public class WorkspaceManager : SingletonMono<WorkspaceManager>
    {
        private readonly List<Facility> facilities = new List<Facility>();
        private int edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
        
        public Facility GetFacility(int x, int y)
        {
            x = ((x % edgeLength) + edgeLength) % edgeLength; y = ((y % edgeLength) + edgeLength) % edgeLength;
            return facilities[x + y*edgeLength];
        }
        public bool TrySetFacility(int x, int y, FacilityType newFacility)
        {
            x = ((x % edgeLength) + edgeLength) % edgeLength; y = ((y % edgeLength) + edgeLength) % edgeLength;
            var index = x + y * edgeLength;
            var oldFacility = facilities[index];
            if (!FacilityFactory.CanBuildOn(newFacility, oldFacility.Type)) return false;

            Destroy(facilities[index].gameObject);
            facilities[index] = FacilityFactory.GetInstanceByType(newFacility, x, y);
            return true;
        }
        public bool Resize(int newEdgeLength)
        {
            if (newEdgeLength < 1 || newEdgeLength > GlobalConsts.MaxWorkspaceEdgeLength) return false;
            edgeLength = newEdgeLength;
            var totalCount = newEdgeLength * newEdgeLength;
            facilities.ForEach(Destroy); facilities.Clear();
            if (facilities.Capacity < totalCount) facilities.Capacity = totalCount;
            for (var i = 0; i < totalCount ; i++) 
                facilities.Add(FacilityFactory.CreateEmpty(i % newEdgeLength, i / newEdgeLength));
            return true;
        }

        private void Start()
        {
            ResourceManager<GameObject>.AddSearchPath("Facilities/Prefabs");
            ResourceManager<GameObject>.LoadAll();
            Resize(edgeLength);
        }
    }
}