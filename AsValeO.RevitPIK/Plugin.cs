using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using RoomFinishes.Controls;
using RoomFinishes.Extensions;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace RoomFinishes
{
    [Transaction(TransactionMode.Manual)]
    public class Plugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //! Инфраструктурный код
            var uiApp = commandData.Application;
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            //! Subscribe to the FailuresProcessing Event
            uiApp.Application.FailuresProcessing += FailuresProcessing;
            using (var tx = new Transaction(doc))
            {
                try
                {
                    //! Бизнес логика
                    RepaintNeighbours(uiDoc, tx);

                    //! Unsubscribe to the FailuresProcessing Event
                    uiApp.Application.FailuresProcessing -= FailuresProcessing;

                    //! Return Success
                    return Result.Succeeded;
                }
                catch (OperationCanceledException exceptionCanceled)
                {
                    message = exceptionCanceled.Message;
                    if (tx.HasStarted()) tx.RollBack();

                    //! Unsubscribe to the FailuresProcessing Event
                    uiApp.Application.FailuresProcessing -= FailuresProcessing;
                    return Result.Cancelled;
                }
                catch (Exception ex)
                {
                    //! Unchecked exception cause command failed
                    message =  ex.Message;

                    //! Trace.WriteLine(ex.ToString());
                    if (tx.HasStarted()) tx.RollBack();

                    //! Unsubscribe to the FailuresProcessing Event
                    uiApp.Application.FailuresProcessing -= FailuresProcessing;
                    return Result.Failed;
                }
            }
        }

        private static void RepaintNeighbours(UIDocument uiDoc, Transaction tx)
        {
            //! Инфраструктурный код
            var doc = uiDoc.Document;
            tx.Start("tName");
            var userControl = new PluginControl();
            userControl.InitializeComponent();
            if (userControl.ShowDialog() == true)
            {
                //! Берем элементы документа категории помещение
                var elems = new FilteredElementCollector(doc);
                elems.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Rooms));

                //! Из них берем только помещения относящиеся к квартирам
                var apartmentRooms = elems.Where(x => x.GetParam("ROM_Зона").Contains("Квартира")).ToList();

                //! Группируем по необходимым параметрам
                var groupedRooms = apartmentRooms.GroupBy(x =>
                    x.GetParam("BS_Этаж") + x.GetParam("BS_Блок") + x.GetParam("ROM_Подзона"));

                //! Меняем цвет помещений, где это необходимо
                foreach (var room in groupedRooms.SelectMany(x => x.ToList().SelectRepaintRequired()))
                    room.GetParameters("ROM_Подзона_Index").FirstOrDefault()
                        ?.Set(room.GetParam("ROM_Расчетная_подзона_ID") + ".Полутон");
                tx.Commit();
            }
            else
            {
                tx.RollBack();
            }
        }

        private static void FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            var failuresAccessor = e.GetFailuresAccessor();
            var failures = failuresAccessor.GetFailureMessages();
            if (failures.Count == 0) return;
            foreach (var f in failures)
            {
                var id = f.GetFailureDefinitionId();
                if (id != BuiltInFailures.JoinElementsFailures.CannotJoinElementsError) return;
                failuresAccessor.ResolveFailure(f);
                e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
                return;
            }
        }
    }
}