using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class DadrasAttractor : GH_Component
    {
        public DadrasAttractor()
          : base("DadrasAttractor", "LA",
            "Construct a DadrasAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(1, 1, 1));
            pManager.AddNumberParameter("Rou", "ρ", "Rou", GH_ParamAccess.item, 3);
            pManager.AddNumberParameter("Sigma", "σ", "Sigma", GH_ParamAccess.item, 2.7);
            pManager.AddNumberParameter("Gama", "γ", "Gama", GH_ParamAccess.item, 1.7);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("Epsilon", "ε", "Epsilon", GH_ParamAccess.item, 9);
            pManager.AddNumberParameter("DeltaT", "Δt", "DeltaT", GH_ParamAccess.item, 0.01);
            pManager.AddIntegerParameter("Iterations", "I", "Number of  iterations", GH_ParamAccess.item, 1000);

        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddPointParameter("Points", "P", "DadrasAttractor", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "C", "DadrasAttractor", GH_ParamAccess.item);

            //pManager.HideParameter(0);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Point3d StartPoint = Point3d.Origin;
            double Rou = 0.0;
            double Sigma = 0.0;
            double Gama = 0.0;
            double Delta = 0.0;
            double Epsilon = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Rou)) return;
            if (!DA.GetData(2, ref Sigma)) return;
            if (!DA.GetData(3, ref Gama)) return;
            if (!DA.GetData(4, ref Delta)) return;
            if (!DA.GetData(5, ref Epsilon)) return;
            if (!DA.GetData(6, ref DeltaT)) return;
            if (!DA.GetData(7, ref Iterations)) return;

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
            List<Point3d> LorenzOscillatorPoints = GenerateDadrasAttractor(StartPoint, Rou, Sigma, Gama, Delta, Epsilon, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)LorenzOscillatorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(LorenzOscillatorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateDadrasAttractor(Point3d StartPoint, double Rou, double Sigma, double Gama, double Delta, double Epsilon, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = y-Rou*x+Sigma*y*z;
                double dy = Gama*y-x*z+z;
                double dz = Delta*x*y-Epsilon*z;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("1D7E9A4C-3F8B-4A2C-9D6E-5B1A8C4F7E3D");
    }
}