// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Routers;

namespace DMX.WebUI3.Components.Models;

/// <summary>
/// Adaptation of <see cref="OrthogonalRouter"/> for Entity Anchors.
/// </summary>
public class OrthoAnchorTestRouter : Router
{
    private readonly Router _fallbackRouter;

    private double _shapeMargin;

    private double _globalMargin;

    public OrthoAnchorTestRouter(double shapeMargin = 10.0, double globalMargin = 50.0, Router? fallbackRouter = null)
    {
        _shapeMargin = shapeMargin;
        _globalMargin = globalMargin;
        _fallbackRouter = fallbackRouter ?? new NormalRouter();
    }

    public override BPoint[] GetRoute(Diagram diagram, BaseLinkModel link)
    {
        if (!link.IsAttached)
        {
            return _fallbackRouter.GetRoute(diagram, link);
        }

        if (!(link.Source is EntityAnchor childAnchor))
        {
            return _fallbackRouter.GetRoute(diagram, link);
        }

        if (!(link.Target is EntityAnchor parentAnchor))
        {
            return _fallbackRouter.GetRoute(diagram, link);
        }

        //PortModel port = null!; // singlePortAnchor.Port;
        //PortModel port2 = null!; // singlePortAnchor2.Port;


        if (parentAnchor == null || parentAnchor.Node.Size == null)
        {
            return _fallbackRouter.GetRoute(diagram, link);
        }

        double num = _shapeMargin;
        double globalMargin = _globalMargin;
        HashSet<BPoint> hashSet = new HashSet<BPoint>();
        List<double> list = new List<double>();
        List<double> list2 = new List<double>();

        //PortAlignment alignment = port.Alignment;
        PortAlignment alignment = childAnchor.GetAlignment(link, []);
        bool flag = IsVerticalSide(alignment);
        //var flag = childAnchor.IsVertical(link, null);

        //PortAlignment alignment2 = port2.Alignment;
        PortAlignment alignment2 = parentAnchor.GetAlignment(link, []);
        bool flag2 = IsVerticalSide(alignment2);
        //var flag2 = parentAnchor.IsVertical(link, null);

        //BPoint portPositionBasedOnAlignment = Router.GetPortPositionBasedOnAlignment(port);
        //BPoint portPositionBasedOnAlignment2 = Router.GetPortPositionBasedOnAlignment(port2);
        BPoint portPositionBasedOnAlignment = childAnchor.GetPosition(link, []) ?? BPoint.Zero;
        BPoint portPositionBasedOnAlignment2 = parentAnchor.GetPosition(link, []) ?? BPoint.Zero;


        //Rectangle bounds = port.Parent.GetBounds(includePorts: true);
        //Rectangle bounds2 = port2.Parent.GetBounds(includePorts: true);
        Rectangle bounds = childAnchor.Node.GetBounds(includePorts: true)!;
        Rectangle bounds2 = parentAnchor.Node.GetBounds(includePorts: true)!;

        Rectangle rectangle = bounds.Inflate(num, num);
        Rectangle rectangle2 = bounds2.Inflate(num, num);
        if (rectangle.Intersects(rectangle2))
        {
            num = 0.0;
            rectangle = bounds;
            rectangle2 = bounds2;
        }

        Rectangle bounds3 = rectangle.Union(rectangle2).Inflate(globalMargin, globalMargin);
        list.Add(rectangle.Left);
        list.Add(rectangle.Right);
        list2.Add(rectangle.Top);
        list2.Add(rectangle.Bottom);
        list.Add(rectangle2.Left);
        list.Add(rectangle2.Right);
        list2.Add(rectangle2.Top);
        list2.Add(rectangle2.Bottom);
        (flag ? list : list2).Add(flag ? portPositionBasedOnAlignment.X : portPositionBasedOnAlignment.Y);
        (flag2 ? list : list2).Add(flag2 ? portPositionBasedOnAlignment2.X : portPositionBasedOnAlignment2.Y);
        hashSet.Add(GetOriginSpot(portPositionBasedOnAlignment, alignment, num));
        hashSet.Add(GetOriginSpot(portPositionBasedOnAlignment2, alignment2, num));
        list.Sort();
        list2.Sort();
        HashSet<BPoint> other = GridToSpots(RulersToGrid(list, list2, bounds3), new Rectangle[2] { rectangle, rectangle2 });
        hashSet.UnionWith(other);
        List<double> list3 = hashSet.Select((BPoint p) => p.Y).Distinct().ToList();
        List<double> list4 = hashSet.Select((BPoint p) => p.X).Distinct().ToList();
        list3.Sort();
        list4.Sort();
        Dictionary<BPoint, Node> dictionary = hashSet.ToDictionary((BPoint p) => p, (BPoint p) => new Node(p));
        for (int i = 0; i < list3.Count; i++)
        {
            for (int j = 0; j < list4.Count; j++)
            {
                BPoint key = new BPoint(list4[j], list3[i]);
                if (!dictionary.ContainsKey(key))
                {
                    continue;
                }

                if (j > 0)
                {
                    BPoint key2 = new BPoint(list4[j - 1], list3[i]);
                    if (dictionary.ContainsKey(key2))
                    {
                        dictionary[key2].ConnectedTo.Add(dictionary[key]);
                        dictionary[key].ConnectedTo.Add(dictionary[key2]);
                    }
                }

                if (i > 0)
                {
                    BPoint key3 = new BPoint(list4[j], list3[i - 1]);
                    if (dictionary.ContainsKey(key3))
                    {
                        dictionary[key3].ConnectedTo.Add(dictionary[key]);
                        dictionary[key].ConnectedTo.Add(dictionary[key3]);
                    }
                }
            }
        }

        Node start = dictionary[GetOriginSpot(portPositionBasedOnAlignment, alignment, num)];
        Node goal = dictionary[GetOriginSpot(portPositionBasedOnAlignment2, alignment2, num)];
        IReadOnlyList<BPoint> path = AStarPathfinder.GetPath(start, goal);
        if (path.Count > 0)
        {
            return path.ToArray();
        }

        return _fallbackRouter.GetRoute(diagram, link);
    }

    private static Grid RulersToGrid(List<double> verticals, List<double> horizontals, Rectangle bounds)
    {
        Grid grid = new Grid();
        verticals.Sort();
        horizontals.Sort();
        double left = bounds.Left;
        double top = bounds.Top;
        int num = 0;
        int num2 = 0;
        foreach (double horizontal in horizontals)
        {
            foreach (double vertical in verticals)
            {
                grid.Set(num2, num++, new Rectangle(left, top, vertical, horizontal));
                left = vertical;
            }

            grid.Set(num2, num, new Rectangle(left, top, bounds.Right, horizontal));
            left = bounds.Left;
            top = horizontal;
            num = 0;
            num2++;
        }

        left = bounds.Left;
        foreach (double vertical2 in verticals)
        {
            grid.Set(num2, num++, new Rectangle(left, top, vertical2, bounds.Bottom));
            left = vertical2;
        }

        grid.Set(num2, num, new Rectangle(left, top, bounds.Right, bounds.Bottom));
        return grid;
    }

    private static HashSet<BPoint> GridToSpots(Grid grid, Rectangle[] obstacles)
    {
        Rectangle[] obstacles2 = obstacles;
        HashSet<BPoint> hashSet = new HashSet<BPoint>();
        foreach (KeyValuePair<double, Dictionary<double, Rectangle>> datum in grid.Data)
        {
            datum.Deconstruct(out var key, out var value);
            double num = key;
            Dictionary<double, Rectangle> dictionary = value;
            bool flag = num == 0.0;
            bool flag2 = num == grid.Rows - 1.0;
            foreach (KeyValuePair<double, Rectangle> item in dictionary)
            {
                item.Deconstruct(out key, out var value2);
                double num2 = key;
                Rectangle rectangle = value2;
                bool flag3 = num2 == 0.0;
                bool flag4 = num2 == grid.Columns - 1.0;
                bool num3 = flag3 && flag;
                bool flag5 = flag && flag4;
                bool flag6 = flag2 && flag4;
                bool flag7 = flag2 && flag3;
                if (num3 || flag5 || flag6 || flag7)
                {
                    AddIfNotInsideObstacles(hashSet, rectangle.NorthWest);
                    AddIfNotInsideObstacles(hashSet, rectangle.NorthEast);
                    AddIfNotInsideObstacles(hashSet, rectangle.SouthWest);
                    AddIfNotInsideObstacles(hashSet, rectangle.SouthEast);
                    continue;
                }

                if (flag)
                {
                    AddIfNotInsideObstacles(hashSet, rectangle.NorthWest);
                    AddIfNotInsideObstacles(hashSet, rectangle.North);
                    AddIfNotInsideObstacles(hashSet, rectangle.NorthEast);
                    continue;
                }

                if (flag2)
                {
                    AddIfNotInsideObstacles(hashSet, rectangle.SouthEast);
                    AddIfNotInsideObstacles(hashSet, rectangle.South);
                    AddIfNotInsideObstacles(hashSet, rectangle.SouthWest);
                    continue;
                }

                if (flag3)
                {
                    AddIfNotInsideObstacles(hashSet, rectangle.NorthWest);
                    AddIfNotInsideObstacles(hashSet, rectangle.West);
                    AddIfNotInsideObstacles(hashSet, rectangle.SouthWest);
                    continue;
                }

                if (flag4)
                {
                    AddIfNotInsideObstacles(hashSet, rectangle.NorthEast);
                    AddIfNotInsideObstacles(hashSet, rectangle.East);
                    AddIfNotInsideObstacles(hashSet, rectangle.SouthEast);
                    continue;
                }

                AddIfNotInsideObstacles(hashSet, rectangle.NorthWest);
                AddIfNotInsideObstacles(hashSet, rectangle.North);
                AddIfNotInsideObstacles(hashSet, rectangle.NorthEast);
                AddIfNotInsideObstacles(hashSet, rectangle.East);
                AddIfNotInsideObstacles(hashSet, rectangle.SouthEast);
                AddIfNotInsideObstacles(hashSet, rectangle.South);
                AddIfNotInsideObstacles(hashSet, rectangle.SouthWest);
                AddIfNotInsideObstacles(hashSet, rectangle.West);
                AddIfNotInsideObstacles(hashSet, rectangle.Center);
            }
        }

        return hashSet;
        void AddIfNotInsideObstacles(HashSet<BPoint> list, BPoint p)
        {
            if (!IsInsideObstacles(p))
            {
                list.Add(p);
            }
        }

        bool IsInsideObstacles(BPoint p)
        {
            Rectangle[] array = obstacles2;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].ContainsPoint(p))
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static bool IsVerticalSide(PortAlignment alignment)
    {
        if (alignment != 0)
        {
            return alignment == PortAlignment.Bottom;
        }

        return true;
    }

    private static BPoint GetOriginSpot(BPoint p, PortAlignment alignment, double shapeMargin)
    {
        return alignment switch
        {
            PortAlignment.Top => p.Add(0.0, 0.0 - shapeMargin),
            PortAlignment.Right => p.Add(shapeMargin, 0.0),
            PortAlignment.Bottom => p.Add(0.0, shapeMargin),
            PortAlignment.Left => p.Add(0.0 - shapeMargin, 0.0),
            _ => throw new NotImplementedException(),
        };
    }

    internal class Node
    {
        public BPoint Position { get; }

        public List<Node> ConnectedTo { get; }

        public double Cost { get; internal set; }

        public Node? Parent { get; internal set; }

        public Node(BPoint position)
        {
            Position = position;
            ConnectedTo = new List<Node>();
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Node node))
            {
                return false;
            }

            return Position.Equals(node.Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    internal class Grid
    {
        public Dictionary<double, Dictionary<double, Rectangle>> Data { get; }

        public double Rows { get; private set; }

        public double Columns { get; private set; }

        public Grid()
        {
            Data = new Dictionary<double, Dictionary<double, Rectangle>>();
        }

        public void Set(double row, double column, Rectangle rectangle)
        {
            Rows = Math.Max(Rows, row + 1.0);
            Columns = Math.Max(Columns, column + 1.0);
            if (!Data.ContainsKey(row))
            {
                Data.Add(row, new Dictionary<double, Rectangle>());
            }

            Data[row].Add(column, rectangle);
        }
    }

    internal static class AStarPathfinder
    {
        public static IReadOnlyList<BPoint> GetPath(Node start, Node goal)
        {
            PriorityQueue<Node, double> priorityQueue = new PriorityQueue<Node, double>();
            priorityQueue.Enqueue(start, 0.0);
            while (priorityQueue.Count > 0)
            {
                Node node = priorityQueue.Dequeue();
                if (node.Equals(goal))
                {
                    break;
                }

                foreach (Node item in node.ConnectedTo)
                {
                    double num = node.Cost + 1.0;
                    if (node.Parent != null && IsChangeOfDirection(node.Parent.Position, node.Position, item.Position))
                    {
                        num *= num;
                        num *= num;
                    }

                    if (item.Cost == 0.0 || num < item.Cost)
                    {
                        item.Cost = num;
                        double priority = num + Heuristic(item.Position, goal.Position);
                        priorityQueue.Enqueue(item, priority);
                        item.Parent = node;
                    }
                }
            }

            List<BPoint> list = new List<BPoint>();
            Node parent = goal.Parent;
            while (parent != null && parent != start)
            {
                list.Insert(0, parent.Position);
                parent = parent.Parent;
            }

            if (parent == start)
            {
                list = SimplifyPath(list);
                if (list.Count > 2)
                {
                    List<BPoint> list2 = list;
                    BPoint prev = list2[list2.Count - 2];
                    List<BPoint> list3 = list;
                    if (AreOnSameLine(prev, list3[list3.Count - 1], goal.Position))
                    {
                        list.RemoveAt(list.Count - 1);
                    }

                    if (AreOnSameLine(start.Position, list[0], list[1]))
                    {
                        list.RemoveAt(0);
                    }
                }

                return list;
            }

            return Array.Empty<BPoint>();
        }

        private static bool AreOnSameLine(BPoint prev, BPoint curr, BPoint next)
        {
            if (prev.X != curr.X || curr.X != next.X)
            {
                if (prev.Y == curr.Y)
                {
                    return curr.Y == next.Y;
                }

                return false;
            }

            return true;
        }

        private static List<BPoint> SimplifyPath(List<BPoint> path)
        {
            for (int num = path.Count - 2; num > 0; num--)
            {
                BPoint prev = path[num + 1];
                BPoint curr = path[num];
                BPoint next = path[num - 1];
                if (AreOnSameLine(prev, curr, next))
                {
                    path.RemoveAt(num);
                }
            }

            return path;
        }

        private static bool IsChangeOfDirection(BPoint a, BPoint b, BPoint c)
        {
            if (a.X == b.X && b.X != c.X)
            {
                return true;
            }

            if (a.Y == b.Y && b.Y != c.Y)
            {
                return true;
            }

            return false;
        }

        private static double Heuristic(BPoint a, BPoint b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }

}
