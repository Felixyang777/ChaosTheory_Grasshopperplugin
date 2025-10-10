using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Render.ChangeQueue;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class zbottom : GH_Component
    {
        public zbottom()
          : base("zbottom", "AA",
            "Construct a zbottom.",
            "ChaosTheory", "zbottom")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "m", "mesh", GH_ParamAccess.item);


        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

          //  pManager.AddNumberParameter("n", "n", "n", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "LorenzOscillator", GH_ParamAccess.list);


            //pManager.HideParameter(0);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Point3d StartPoint = Point3d.Origin;
            Rhino.Geometry.Mesh mesh=new Rhino.Geometry.Mesh() ;

            if (!DA.GetData(0, ref mesh)) return;

            List<Point3d> vertices = new List<Point3d>();

            for (int i = 0; i < mesh.TopologyVertices.Count; i++)
            {
                Vector3d vdown = new Vector3d(0, 0, -0.001);
                Vector3d wup = new Vector3d(0, 0, 0.001);
                Point3d p = mesh.Vertices[i];

                if ((mesh.IsPointInside(p + vdown, 0, true) == false) &&
                  (mesh.IsPointInside(p + wup, 0, true) == true))
                {
                    int[] neighbors;
                    neighbors = mesh.TopologyVertices.ConnectedTopologyVertices(i);
                    bool bottom = true;

                    foreach (int index in neighbors)
                    {
                        if (mesh.Vertices[i].Z > mesh.Vertices[index].Z)
                        {
                            bottom = false;
                            break;
                        }
                    }
                    if (bottom == true)  
                    {
                        vertices.Add(mesh.Vertices[i]);
                    }
                }
            }

            IEnumerable __enum_points = (IEnumerable)vertices;
            DA.SetDataList(0, __enum_points);


        }

        List<Point3d> newpoints;
        Point3d point;


        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("6A7E9B4C-1F3D-4A8D-9C6E-5B3A7D1F4E0C");
    }
}