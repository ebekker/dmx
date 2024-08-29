// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3.Components;

public partial class NewRelationshipDetails : IDisposable
{
    [Parameter] public AppDbContext DB { get; set; } = default!;
    [Parameter] public DmxRelationship Relationship { get; set; } = default!;
    [Parameter] public AppSignal<DPoint>? DragSignal { get; set; }
    [Parameter] public AppSignal<DSize>? ResizeSignal { get; set; }

    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;

    [Inject] private AppServices Services { get; set; } = default!;

    List<DmxEntity>? _entities;
    List<string>? _allRelNames;
    DmxEntity? _parent;
    DmxEntity? _child;
    string? _suggestedName;
    readonly List<RelPair> _relPairs = new();

    class RelPair
    {
        public required DmxAttribute Parent { get; init; }
        public DmxAttribute? Child { get; set; }
        public string? NewChild { get; set; }
    }

    protected override async Task OnInitializedAsync()
    {
        if (DragSignal != null)
            DragSignal.OnSignal += DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal += ResizeSignal_OnSignal;

        _entities = await DB.Entities
            .Include(x => x.Attributes)
                .ThenInclude(x => x.Domain)
            .ToListAsync();
        _allRelNames = await DB.Relationships
            .Select(x => x.Name)
            .ToListAsync();
    }

    public void Dispose()
    {
        if (DragSignal != null)
            DragSignal.OnSignal -= DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal -= ResizeSignal_OnSignal;
    }

    void DoOK()
    {
        if (_parent == null || _child == null)
            return;

        Relationship.Parent = _parent;
        Relationship.Child = _child;
        Relationship.Attributes ??= new();

        foreach (var rp in _relPairs)
        {
            var ca = rp.Child;
            if (ca == null)
            {
                ca = new()
                {
                    Entity = _child,
                    Name = rp.NewChild ?? "NewAtt",
                    Domain = rp.Parent.Domain,
                };
                _child.Attributes ??= new();
                _child.Attributes.Add(ca);
            }

            var relPair = new DmxRelationshipPair
            {
                Relationship = Relationship,
                Parent = rp.Parent,
                Child = ca,
            };
            Relationship.Attributes.Add(relPair);
        }

        DialogSvc.Close(true);
    }

    void UpdateName()
    {
        if (_parent != null && _child != null)
        {
            var oldSName = _suggestedName;
            _suggestedName = $"{_child.Name}{_parent.Name}";
            _suggestedName = Services.Model.NextAvailableName(_allRelNames!, _suggestedName, start: null);
            if (string.IsNullOrWhiteSpace(Relationship.Name)
                || string.Equals(Relationship.Name, oldSName))
            {
                Relationship.Name = _suggestedName;
            }
        }
    }

    void AfterParentUpdate()
    {
        _relPairs.Clear();
        if (_parent != null)
        {
            foreach (var a in _parent.Attributes.Where(x => x.IsPrimaryKey))
            {
                _relPairs.Add(new()
                {
                    Parent = a,
                });
            }
            AfterChildUpdate();
        }
    }

    void AfterChildUpdate()
    {
        if (_parent != null && _child != null)
        {
            var childNames = _child.Attributes
                .Select(x => x.Name)
                .ToList();

            foreach (var rp in _relPairs)
            {
                rp.NewChild = Services.Model.NextAvailableName(childNames,
                    rp.Parent.Name, start: null);
            }
            UpdateName();
        }
    }

    private void DragSignal_OnSignal(object? sender, DPoint arg)
    {
        // Do we want to save position?
        AppState.RelationshipDetailsPoint = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }

    private void ResizeSignal_OnSignal(object? sender, DSize arg)
    {
        AppState.RelationshipDetailsSize = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }


    string? columnEditing;

    bool IsEditing(string columnName, DmxRelationshipPair att)
    {
        // Comparing strings is quicker than checking the
        // contents of a List, so let the property check fail first.
        return string.Equals(columnEditing, columnName);
    }

    string IsEdited(RadzenDataGridColumn<DmxAttribute> column, DmxAttribute order)
    {
        //// In a real scenario, you might use IRevertibleChangeTracking to check the current column
        ////  against a list of the object's edited fields.
        //return editedFields.Where(c => c.Key == order.OrderID && c.Value == column.Property).Any() ?
        //    "table-cell-edited" :
        //    string.Empty;
        return string.Empty;
    }

    void OnCellClick(DataGridCellMouseEventArgs<DmxRelationshipPair> args)
    {
        //// Record the previous edited field, if you're not using IRevertibleChangeTracking to track object changes
        //if (ordersToUpdate.Any())
        //{
        //    editedFields.Add(new(ordersToUpdate.First().OrderID, columnEditing));
        //}

        //// This sets which column is currently being edited.
        //columnEditing = args.Column.Property;

        //// This triggers a save on the previous edit. This can be removed if you are going to batch edits through another method.
        //if (ordersToUpdate.Any())
        //{
        //    OnUpdateRow(ordersToUpdate.First());
        //}

        //// This sets the Item to be edited.
        //EditRow(args.Data);
    }

    void EditRow(DmxRelationshipPair att)
    {
        //Reset();

        //ordersToUpdate.Add(order);
    }

    void OnUpdateRow(DmxRelationshipPair att)
    {
        //Reset(order);

        //dbContext.Update(order);

        //dbContext.SaveChanges();

        // If you were doing row-level edits and handling RowDeselect, you could use the line below to
        // clear edits for the current record.

        //editedFields = editedFields.Where(c => c.Key != order.OrderID).ToList();
    }

    public static async Task<bool?> ShowAsync(DialogService dlg,
        AppDbContext db, DmxRelationship relationship,
        DPoint? initPoint = null, DSize? initSize = null)
    {
        var dragSignal = new AppSignal<DPoint>();
        var resizeSignal = new AppSignal<DSize>();
        var @params = new Dictionary<string, object>
        {
            [nameof(DB)] = db,
            [nameof(Relationship)] = relationship,
            [nameof(DragSignal)] = dragSignal,
            [nameof(ResizeSignal)] = resizeSignal,
        };
        var opts = new DialogOptions
        {
            Draggable = true,
            Resizable = true,
            Drag = x => dragSignal.FireSignal(dragSignal, x),
            Resize = x => resizeSignal.FireSignal(resizeSignal, x),
        };
        if (initPoint != null)
        {
            opts.Left = initPoint.Value.X + "px";
            opts.Top = initPoint.Value.Y + "px";
        }
        if (initSize != null)
        {
            opts.Width = initSize.Value.Width + "px";
            opts.Height = initSize.Value.Height + "px";
        }

        var result = (bool?)await dlg.OpenAsync<NewRelationshipDetails>(
            "New Relationship", @params, opts);

        return result;
    }
}
