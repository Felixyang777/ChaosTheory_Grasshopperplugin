using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class CoulletAttractor : GH_Component
    {
        public CoulletAttractor()
          : base("CoulletAttractor", "AA",
            "Construct a CoulletAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(1, 0, 0));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 0.8);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, -1.1);
            pManager.AddNumberParameter("Sigma", "ς", "Sigma", GH_ParamAccess.item, -0.45);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("DeltaT", "Δt", "DeltaT", GH_ParamAccess.item, 0.001);
            pManager.AddIntegerParameter("Iterations", "I", "Number of  iterations", GH_ParamAccess.item, 100000);

        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddPointParameter("Points", "P", "LorenzOscillator", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "C", "LorenzOscillator", GH_ParamAccess.item);

            //pManager.HideParameter(0);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Point3d StartPoint = Point3d.Origin;
            double Alpha = 0.0;
            double Beta = 0.0;
            double Sigma = 0.0;
            double Delta = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Alpha)) return;
            if (!DA.GetData(2, ref Beta)) return;
            if (!DA.GetData(3, ref Sigma)) return;
            if (!DA.GetData(4, ref Delta)) return;
            if (!DA.GetData(5, ref DeltaT)) return;
            if (!DA.GetData(6, ref Iterations)) return;

            if (DeltaT <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "DeltaT must be positive");
                return;
            }

            if (Iterations <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Iterations must be positive");
                return;
            }
            List<Point3d> CoulletAttractorPoints = GenerateCoulletAttractor(StartPoint, Alpha, Beta, Sigma, Delta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)CoulletAttractorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(CoulletAttractorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateCoulletAttractor(Point3d StartPoint, double Alpha, double Beta, double Sigma, double Delta,  double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = y;
                double dy = z;
                double dz = Alpha*x+Beta*y+Sigma*z+Delta*Math.Pow(x,3);

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("4E9A7C1D-3F8B-4A2C-9D6E-5B1A8C4F7E3D");
    }
}