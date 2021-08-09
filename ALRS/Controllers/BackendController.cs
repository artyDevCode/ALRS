using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Data;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;

namespace ALRSSystem.Controllers
{
    //[SharePointContextFilter]
    public class BackendController : Controller
    {
        //
        // GET: /Scheduler/
        
        public ActionResult Day()
        {
            return new Dpc().CallBack(this);
        }

        public ActionResult Week()
        {
            return new Dpc().CallBack(this);
        }

        public ActionResult Month()
        {
            return new Dpm().CallBack(this);
        }

        class Dpc : DayPilotCalendar
        {
            ALRSDB _db = new ALRSDB();
            protected override void OnInit(InitArgs e)
            {
                Update(CallBackUpdateType.Full);
            }

            protected override void OnEventResize(EventResizeArgs e)
            {
                //new EventManager().EventMove(e.Id, e.NewStart, e.NewEnd);
                Update();
            }

            protected override void OnEventMove(EventMoveArgs e)
            {
                new EventManager().EventMove(e.Id, e.NewStart, e.NewEnd);
                Update();
            }

            protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
            {
                //new EventManager().EventCreate(e.Start, e.End, "New event");
                Update();
            }

            protected override void OnBeforeEventRender(BeforeEventRenderArgs e)
            {
                e.Areas.Add(new Area().Right(3).Top(3).Width(15).Height(15).CssClass("event_action_delete").JavaScript("switcher.active.control.commandCallBack('delete', {'e': e});"));
                e.BackgroundColor = e.Tag["Color"];
            }

            protected override void OnCommand(CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate":
                        StartDate = (DateTime)e.Data["day"];
                        Update(CallBackUpdateType.Full);
                        break;
                    case "refresh":
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                    case "delete":
                        new EventManager().EventDelete((string)e.Data["e"]["id"]);
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                }
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                Events = _db.ALRS
                  .ToList()
                  .Where(r => r.DocumentDeleted == false)
                  .Select(r => new CalendarData
                  {
                      Id = r.ID,
                      Start = r.ALRSStartDate,
                      Text = r.ALRSName,
                      End = r.ALRSEndDate,
                      //Resource = r.Resource,
                      Allday = Convert.ToBoolean(r.ALRSAllDayEvent),
                      Color = r.ALRSBackColor
                      //  Text = r.Text
                  });

                DataIdField = "Id";
                DataTextField = "Text";
                DataStartField = "Start";
                DataEndField = "End";
                DataTagFields = "Color, Allday";
                DataAllDayField = "allday";
                ShowAllDayEvents= true;

                //Events = new EventManager().FilteredData(StartDate, StartDate.AddDays(Days)).AsEnumerable();
                //DataIdField = "ID";
                //DataTextField = "ALRSName";
                //DataStartField = "ALRSStartDate";
                //DataEndField = "ALRSEndDate";
                //DataAllDayField = "true";
                //DataTagFields = "ALRSBackColor,ALRSAllDayEvent";
                }
        }


        class Dpm : DayPilotMonth
        {

            ALRSDB _db = new ALRSDB();
            protected override void OnInit(DayPilot.Web.Mvc.Events.Month.InitArgs e)
            {
                DataIdField = "Id";
                DataTextField = "Text";
                DataStartField = "Start";
                DataEndField = "End";
                DataTagFields = "Color,AlldayEvent";

                //DataIdField = "ID";
                //DataTextField = "ALRSName";
                //DataStartField = "ALRSStartDate";
                //DataEndField = "ALRSEndDate";
                //DataTagFields = "ALRSBackColor,ALRSAllDayEvent";

                Update(CallBackUpdateType.Full);  //Update();
                UpdateWithMessage("Welcome!", CallBackUpdateType.Full);
            }

            protected override void OnEventResize(DayPilot.Web.Mvc.Events.Month.EventResizeArgs e)
            {
                //new EventManager().EventMove(e.Id, e.NewStart, e.NewEnd);
                Update();
            }

            protected override void OnEventMove(DayPilot.Web.Mvc.Events.Month.EventMoveArgs e)
            {
                //new EventManager().EventMove(e.Id, e.NewStart, e.NewEnd);
                Update();
            }

            protected override void OnTimeRangeSelected(DayPilot.Web.Mvc.Events.Month.TimeRangeSelectedArgs e)
            {
                //new EventManager().EventCreate(e.Start, e.End, "New event");
                Update();
            }

            protected override void OnBeforeEventRender(DayPilot.Web.Mvc.Events.Month.BeforeEventRenderArgs e)
            {
                e.Areas.Add(new Area().Right(3).Top(3).Width(15).Height(15).CssClass("event_action_delete").JavaScript("switcher.active.control.commandCallBack('delete', {'e': e});"));
                e.BackgroundColor = e.Tag["Color"];
            }

            protected override void OnCommand(DayPilot.Web.Mvc.Events.Month.CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate":
                        StartDate = (DateTime)e.Data["day"];
                        Update(CallBackUpdateType.Full);
                        break;
                    case "refresh":
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                    case "delete":
                        new EventManager().EventDelete((string)e.Data["e"]["id"]);
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                }
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                Events = _db.ALRS
                  .ToList()
                  .Where(r => r.DocumentDeleted == false)
                  .Select(r => new CalendarData
                  {
                      Id = r.ID,
                      Start = r.ALRSStartDate,
                      Text = r.ALRSName,
                      End = r.ALRSEndDate,
                      //Resource = r.Resource,
                      Allday = Convert.ToBoolean(r.ALRSAllDayEvent),
                      Color = r.ALRSBackColor
                      //  Text = r.Text
                  });

                DataIdField = "Id";
                DataTextField = "Text";
                DataStartField = "Start";
                DataEndField = "End";
                DataTagFields = "Color, Allday";

                //Events = new EventManager().FilteredData(VisibleStart, VisibleEnd).AsEnumerable();
                //DataIdField = "ID";
                //DataTextField = "ALRSName";
                //DataStartField = "ALRSStartDate";
                //DataEndField = "ALRSEndDate";
                //DataTagFields = "ALRSBackColor, ALRSAllDayEvent";

            }
        }


    }
}
