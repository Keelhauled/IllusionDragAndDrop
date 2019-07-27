using System;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace IllusionDragAndDrop.Shared.WinAPI
{
    public class DropTarget : IOleDropTarget
    {
        private IDataObject lastDataObject = null;
        private DragDropEffects lastEffect = DragDropEffects.None;
        private readonly IDropTarget owner;

        public DropTarget(IDropTarget owner)
        {
            Console.WriteLine("DropTarget created");
            this.owner = owner;
        }

#if DEBUG
        ~DropTarget()
        {
            Console.WriteLine("DropTarget destroyed");
        }
#endif

        private DragEventArgs CreateDragEventArgs(object pDataObj, int grfKeyState, POINT pt, int pdwEffect)
        {
            IDataObject data = null;

            if(pDataObj == null)
            {
                data = lastDataObject;
            }
            else
            {
                if(pDataObj is IDataObject)
                {
                    data = (IDataObject)pDataObj;
                }
                else if(pDataObj is IComDataObject)
                {
                    //data = new DataObject(pDataObj);
                    Console.WriteLine("pDataObj is IComDataObject");
                }
                else
                {
                    return null; // Unknown data object interface; we can't work with this so return null
                }
            }

            DragEventArgs drgevent = new DragEventArgs(data, grfKeyState, pt.x, pt.y, (DragDropEffects)pdwEffect, lastEffect);
            lastDataObject = data;
            return drgevent;
        }

        int IOleDropTarget.OleDragEnter(object pDataObj, int grfKeyState, POINTSTRUCT pt, ref int pdwEffect)
        {
            Console.WriteLine("OleDragEnter recieved");
            POINT ptl = new POINT
            {
                x = pt.x,
                y = pt.y
            };
            Console.WriteLine("\t" + (ptl.x) + "," + (ptl.y));
            if(pDataObj == null) Console.WriteLine("OleDragEnter didn't give us a valid data object.");
            DragEventArgs drgevent = CreateDragEventArgs(pDataObj, grfKeyState, ptl, pdwEffect);

            if(drgevent != null)
            {
                owner.OnDragEnter(drgevent);
                pdwEffect = (int)drgevent.Effect;
                lastEffect = drgevent.Effect;
            }
            else
            {
                pdwEffect = (int)DragDropEffects.None;
            }
            return NativeConstants.S_OK;
        }

        int IOleDropTarget.OleDragOver(int grfKeyState, POINTSTRUCT pt, ref int pdwEffect)
        {
            Console.WriteLine("OleDragOver recieved");
            POINT ptl = new POINT
            {
                x = pt.x,
                y = pt.y
            };
            Console.WriteLine("\t" + (ptl.x) + "," + (ptl.y));
            DragEventArgs drgevent = CreateDragEventArgs(null, grfKeyState, ptl, pdwEffect);
            owner.OnDragOver(drgevent);
            pdwEffect = (int)drgevent.Effect;
            lastEffect = drgevent.Effect;
            return NativeConstants.S_OK;
        }

        int IOleDropTarget.OleDragLeave()
        {
            Console.WriteLine("OleDragLeave recieved");
            owner.OnDragLeave(EventArgs.Empty);
            return NativeConstants.S_OK;
        }

        int IOleDropTarget.OleDrop(object pDataObj, int grfKeyState, POINTSTRUCT pt, ref int pdwEffect)
        {
            Console.WriteLine("OleDrop recieved");
            POINT ptl = new POINT
            {
                x = pt.x,
                y = pt.y
            };
            Console.WriteLine("\t" + (ptl.x) + "," + (ptl.y));
            DragEventArgs drgevent = CreateDragEventArgs(pDataObj, grfKeyState, ptl, pdwEffect);

            if(drgevent != null)
            {
                owner.OnDragDrop(drgevent);
                pdwEffect = (int)drgevent.Effect;
            }
            else
            {
                pdwEffect = (int)DragDropEffects.None;
            }

            lastEffect = DragDropEffects.None;
            lastDataObject = null;
            return NativeConstants.S_OK;
        }
    }

    [Flags]
    public enum DragDropEffects
    {
        /// <summary>
        /// The drop target does not accept the data.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// The data is copied to the drop target.
        /// </summary>
        Copy = 0x00000001,

        /// <summary>
        /// The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 0x00000002,

        /// <summary>
        /// The data from the drag source is linked to the drop target.
        /// </summary>
        Link = 0x00000004,

        /// <summary>
        /// Scrolling is about to start or is currently occurring in the drop target.
        /// </summary>
        Scroll = unchecked((int)0x80000000),

        /// <summary>
        /// The data is copied, removed from the drag source, and scrolled in the
        /// drop target. NOTE: Link is intentionally not present in All.
        /// </summary>
        All = Copy | Move | Scroll,
    }
}
