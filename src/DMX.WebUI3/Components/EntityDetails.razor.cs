﻿// DMX.
// Copyright (C) Eugene Bekker.

using System.Drawing;
using Microsoft.EntityFrameworkCore;
using DPoint = System.Drawing.Point;

namespace DMX.WebUI3.Components;

/// <summary>
/// Defines the a dialog box for editing an Entity's details.
/// </summary>
public partial class EntityDetails : IDisposable
{
    [Parameter] public DmxEntity Entity { get; set; } = default!;
    [Parameter] public AppSignal<DPoint>? DragSignal { get; set; }
    [Parameter] public AppSignal<Size>? ResizeSignal { get; set; }

    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (DragSignal != null)
            DragSignal.OnSignal += DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal += ResizeSignal_OnSignal;
    }

    public void Dispose()
    {
        if (DragSignal != null)
            DragSignal.OnSignal -= DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal -= ResizeSignal_OnSignal;
    }

    private void DragSignal_OnSignal(object? sender, DPoint arg)
    {
        // Do we want to save position?
        //AppState.EntityDetailsPoint = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }

    private void ResizeSignal_OnSignal(object? sender, Size arg)
    {
        AppState.EntityDetailsSize = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }


    string? columnEditing;

    bool IsEditing(string columnName, DmxAttribute att)
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

    void OnCellClick(DataGridCellMouseEventArgs<DmxAttribute> args)
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

    void EditRow(DmxAttribute att)
    {
        //Reset();

        //ordersToUpdate.Add(order);
    }

    void OnUpdateRow(DmxAttribute att)
    {
        //Reset(order);

        //dbContext.Update(order);

        //dbContext.SaveChanges();

        // If you were doing row-level edits and handling RowDeselect, you could use the line below to
        // clear edits for the current record.

        //editedFields = editedFields.Where(c => c.Key != order.OrderID).ToList();
    }

    public static async Task ShowAsync(DialogService dlg, DmxEntity entity,
        DPoint? initPoint = null, Size? initSize = null)
    {
        var dragSignal = new AppSignal<DPoint>();
        var resizeSignal = new AppSignal<Size>();
        var @params = new Dictionary<string, object>
        {
            [nameof(Entity)] = entity,
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

        await dlg.OpenAsync<EntityDetails>("Entity", @params, opts);
    }
}
