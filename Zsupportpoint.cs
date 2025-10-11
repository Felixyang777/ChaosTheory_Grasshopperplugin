using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ChaosTheory
{
    public class AdvancedSupportAnalysis : GH_Component
    {
        public AdvancedSupportAnalysis()
          : base("Advanced Support Analysis", "AdvSupport",
            "结合底部点检测和支撑点提取，过滤过近点",
            "ChaosTheory", "Support")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "要分析的网格模型", GH_ParamAccess.item);
            pManager.AddNumberParameter("Grid Size", "GS", "栅格尺寸", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("Offset Distance", "OD", "支撑点偏移距离", GH_ParamAccess.item, 0.2);
            pManager.AddNumberParameter("Filter Distance", "FD", "过滤距离（去除离底部点过近的点）", GH_ParamAccess.item, 1.0);
            pManager.AddPlaneParameter("Base Plane", "BP", "基准平面", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Support Angle", "SA", "支撑角度阈值（度）", GH_ParamAccess.item, 45.0);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Support Mesh", "SM", "需要支撑的网格区域", GH_ParamAccess.item);
            pManager.AddPointParameter("Bottom Points", "BP", "检测到的底部点", GH_ParamAccess.list);
            pManager.AddPointParameter("Grid Points", "GP", "栅格法提取的原始支撑点", GH_ParamAccess.list);
            pManager.AddPointParameter("Filtered Points", "FP", "过滤后的支撑点", GH_ParamAccess.list);
            pManager.AddNumberParameter("Support Area", "SA", "需要支撑的总面积", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Point Count", "PC", "过滤后支撑点数量", GH_ParamAccess.item);
            pManager.AddCurveParameter("Grid Cells", "GC", "栅格单元格", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = new Mesh();
            double gridSize = 2.0;
            double offsetDistance = 0.2;
            double filterDistance = 1.0;
            Plane basePlane = Plane.WorldXY;
            double supportAngle = 45.0;

            if (!DA.GetData(0, ref mesh)) return;
            DA.GetData(1, ref gridSize);
            DA.GetData(2, ref offsetDistance);
            DA.GetData(3, ref filterDistance);
            DA.GetData(4, ref basePlane);
            DA.GetData(5, ref supportAngle);

            if (mesh == null || !mesh.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "输入的网格无效或为空");
                return;
            }

            // 1. 检测支撑区域网格
            Mesh supportMesh = DetectSupportMesh(mesh, supportAngle);

            // 2. 使用 zbottom 算法检测底部点
            List<Point3d> bottomPoints = DetectBottomPoints(mesh);

            // 3. 使用栅格法从支撑网格提取支撑点
            List<Point3d> gridPoints;
            List<Curve> gridCells;
            ExtractGridSupportPoints(supportMesh, basePlane, gridSize, offsetDistance, out gridPoints, out gridCells);

            // 4. 过滤离底部点过近的点
            List<Point3d> filteredPoints = FilterPointsByDistance(gridPoints, bottomPoints, filterDistance);

            // 5. 计算支撑区域总面积
            double supportArea = CalculateMeshArea(supportMesh);

            // 6. 设置输出
            DA.SetData(0, supportMesh);
            DA.SetDataList(1, bottomPoints);
            DA.SetDataList(2, gridPoints);
            DA.SetDataList(3, filteredPoints);
            DA.SetData(4, supportArea);
            DA.SetData(5, filteredPoints.Count);
            DA.SetDataList(6, gridCells);

            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                $"支撑区域面积: {supportArea:F2}, 底部点: {bottomPoints.Count}, " +
                $"栅格点: {gridPoints.Count}, 过滤后: {filteredPoints.Count}");
        }

        /// <summary>
        /// 检测需要支撑的网格区域（基于法线与Z轴夹角）
        /// </summary>
        private Mesh DetectSupportMesh(Mesh mesh, double supportAngle)
        {
            Mesh processedMesh = mesh.DuplicateMesh();
            processedMesh.FaceNormals.ComputeFaceNormals();

            // 创建支撑区域网格
            Mesh supportMesh = new Mesh();
            double thresholdRad = supportAngle * Math.PI / 180.0;
            Vector3d zAxis = new Vector3d(0, 0, -1);

            for (int i = 0; i < processedMesh.Faces.Count; i++)
            {
                MeshFace face = processedMesh.Faces[i];

                if (face.IsTriangle)
                {
                    Point3d v1 = processedMesh.Vertices[face.A];
                    Point3d v2 = processedMesh.Vertices[face.B];
                    Point3d v3 = processedMesh.Vertices[face.C];

                    // 获取法线向量
                    Vector3d normal = processedMesh.FaceNormals[i];
                    normal.Unitize();

                    // 计算法线与Z轴的夹角
                    double dotProduct = normal * zAxis;
                    double angle = Math.Acos(dotProduct);

                    // 判断是否需要支撑
                    bool needsSupport = angle < thresholdRad;

                    if (needsSupport)
                    {
                        // 添加到支撑网格
                        supportMesh.Vertices.Add(v1);
                        supportMesh.Vertices.Add(v2);
                        supportMesh.Vertices.Add(v3);
                        supportMesh.Faces.AddFace(
                            supportMesh.Vertices.Count - 3,
                            supportMesh.Vertices.Count - 2,
                            supportMesh.Vertices.Count - 1
                        );

                        // 着色（红色表示需要支撑）
                        supportMesh.VertexColors.Add(Color.Red);
                        supportMesh.VertexColors.Add(Color.Red);
                        supportMesh.VertexColors.Add(Color.Red);
                    }
                }
            }

            return supportMesh;
        }

        /// <summary>
        /// 检测底部点（基于您的 zbottom 算法）
        /// </summary>
        private List<Point3d> DetectBottomPoints(Mesh mesh)
        {
            List<Point3d> bottomPoints = new List<Point3d>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                Vector3d vdown = new Vector3d(0, 0, -0.001);
                Vector3d wup = new Vector3d(0, 0, 0.001);
                Point3d p = mesh.Vertices[i];

                if ((mesh.IsPointInside(p + vdown, 0, true) == false) &&
                  (mesh.IsPointInside(p + wup, 0, true) == true))
                {
                    int[] neighbors;
                    neighbors = mesh.Vertices.GetConnectedVertices(i);
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
                        bottomPoints.Add(mesh.Vertices[i]);
                    }
                }
            }

            return bottomPoints;
        }

        /// <summary>
        /// 使用栅格法提取支撑点
        /// </summary>
        private void ExtractGridSupportPoints(Mesh supportMesh, Plane basePlane, double gridSize,
            double offsetDistance, out List<Point3d> supportPoints, out List<Curve> gridCells)
        {
            supportPoints = new List<Point3d>();
            gridCells = new List<Curve>();

            if (supportMesh == null || supportMesh.Vertices.Count == 0)
                return;

            // 复制网格以避免修改原始数据
            Mesh mesh = supportMesh.DuplicateMesh();
            mesh.FaceNormals.ComputeFaceNormals();

            // 获取网格的边界框
            BoundingBox bbox = mesh.GetBoundingBox(true);

            List<Point3d> gridCenters = new List<Point3d>();

            // 创建栅格
            CreateGridFromBoundingBox(bbox, basePlane, gridSize, gridCenters, gridCells);

            // 对每个栅格中心点进行射线求交
            foreach (Point3d gridCenter in gridCenters)
            {
                // 创建射线（从下方沿Z轴正方向）
                Point3d rayStart = new Point3d(gridCenter.X, gridCenter.Y, bbox.Min.Z - 10);
                Point3d rayEnd = new Point3d(gridCenter.X, gridCenter.Y, bbox.Max.Z + 10);
                Line ray = new Line(rayStart, rayEnd);

                // 进行射线与网格求交
                Point3d intersectionPoint = FindMeshIntersection(mesh, ray);

                if (intersectionPoint != Point3d.Unset)
                {
                    // 找到最近的网格点以获取法线
                    var closestPoint = mesh.ClosestPoint(intersectionPoint);
                    if (closestPoint != Point3d.Unset)
                    {
                        MeshPoint meshPoint = mesh.ClosestMeshPoint(intersectionPoint, 0.0);
                        if (meshPoint != null)
                        {
                            Vector3d normal = mesh.FaceNormals[meshPoint.FaceIndex];

                            // 应用偏移（沿法线方向）
                            Point3d supportPoint = intersectionPoint - normal * offsetDistance;

                            supportPoints.Add(supportPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据距离过滤点，去除离底部点过近的点
        /// </summary>
        private List<Point3d> FilterPointsByDistance(List<Point3d> gridPoints, List<Point3d> bottomPoints, double minDistance)
        {
            if (bottomPoints.Count == 0)
                return gridPoints;

            List<Point3d> filteredPoints = new List<Point3d>();

            foreach (Point3d gridPoint in gridPoints)
            {
                bool tooClose = false;

                foreach (Point3d bottomPoint in bottomPoints)
                {
                    double distance = gridPoint.DistanceTo(bottomPoint);
                    if (distance < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    filteredPoints.Add(gridPoint);
                }
            }

            return filteredPoints;
        }

        /// <summary>
        /// 计算网格总面积
        /// </summary>
        private double CalculateMeshArea(Mesh mesh)
        {
            double totalArea = 0.0;

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                MeshFace face = mesh.Faces[i];
                if (face.IsTriangle)
                {
                    Point3d v1 = mesh.Vertices[face.A];
                    Point3d v2 = mesh.Vertices[face.B];
                    Point3d v3 = mesh.Vertices[face.C];

                    Vector3d v12 = v2 - v1;
                    Vector3d v13 = v3 - v1;
                    totalArea += Vector3d.CrossProduct(v12, v13).Length * 0.5;
                }
            }

            return totalArea;
        }

        /// <summary>
        /// 查找射线与网格的交点
        /// </summary>
        private Point3d FindMeshIntersection(Mesh mesh, Line ray)
        {
            double closestDistance = double.MaxValue;
            Point3d closestIntersection = Point3d.Unset;

            // 遍历所有面进行求交
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                MeshFace face = mesh.Faces[i];

                if (face.IsTriangle)
                {
                    Point3d v1 = mesh.Vertices[face.A];
                    Point3d v2 = mesh.Vertices[face.B];
                    Point3d v3 = mesh.Vertices[face.C];

                    // 计算三角形平面
                    Plane facePlane = new Plane(v1, v2, v3);

                    // 求射线与平面的交点
                    double rayParameter;
                    if (Rhino.Geometry.Intersect.Intersection.LinePlane(ray, facePlane, out rayParameter))
                    {
                        Point3d potentialIntersection = ray.PointAt(rayParameter);

                        // 检查交点是否在三角形内部
                        if (IsPointInTriangle(potentialIntersection, v1, v2, v3, facePlane))
                        {
                            double distance = ray.From.DistanceTo(potentialIntersection);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestIntersection = potentialIntersection;
                            }
                        }
                    }
                }
            }

            return closestIntersection;
        }

        /// <summary>
        /// 检查点是否在三角形内部
        /// </summary>
        private bool IsPointInTriangle(Point3d point, Point3d v1, Point3d v2, Point3d v3, Plane plane)
        {
            // 将点投影到三角形平面
            Point3d projectedPoint = plane.ClosestPoint(point);

            // 使用重心坐标法判断点是否在三角形内
            double area = Vector3d.CrossProduct(v2 - v1, v3 - v1).Length;
            double alpha = Vector3d.CrossProduct(v2 - projectedPoint, v3 - projectedPoint).Length / area;
            double beta = Vector3d.CrossProduct(v3 - projectedPoint, v1 - projectedPoint).Length / area;
            double gamma = Vector3d.CrossProduct(v1 - projectedPoint, v2 - projectedPoint).Length / area;

            // 允许一定的浮点误差
            double tolerance = 0.0001;
            return Math.Abs(alpha + beta + gamma - 1.0) < tolerance &&
                   alpha >= -tolerance && beta >= -tolerance && gamma >= -tolerance;
        }

        /// <summary>
        /// 从边界框创建栅格
        /// </summary>
        private void CreateGridFromBoundingBox(BoundingBox bbox, Plane basePlane, double gridSize,
            List<Point3d> gridCenters, List<Curve> gridCells)
        {
            // 计算栅格行列数
            int cols = Math.Max(1, (int)Math.Ceiling((bbox.Max.X - bbox.Min.X) / gridSize));
            int rows = Math.Max(1, (int)Math.Ceiling((bbox.Max.Y - bbox.Min.Y) / gridSize));

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    double x = bbox.Min.X + i * gridSize;
                    double y = bbox.Min.Y + j * gridSize;

                    // 确保不超出边界
                    if (x + gridSize > bbox.Max.X || y + gridSize > bbox.Max.Y)
                        continue;

                    Point3d gridCenter = new Point3d(x + gridSize / 2, y + gridSize / 2, basePlane.OriginZ);

                    // 创建栅格单元格
                    Rectangle3d rect = new Rectangle3d(basePlane,
                        new Interval(x, x + gridSize),
                        new Interval(y, y + gridSize));

                    gridCenters.Add(gridCenter);
                    gridCells.Add(rect.ToNurbsCurve());
                }
            }
        }

        protected override Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("9a8b7c6d-5e4f-3a2b-1c0d-9e8f7a6b5c4d");

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}