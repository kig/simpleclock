using System;
using Gtk;
using Cairo;

class Simpleclock : DrawingArea
{

  private static Gtk.Window win = null;

  /**
    The Main method inits the Gtk application and creates a Simpleclock instance
    to run.
  */
  static void Main ()
  {
    Application.Init ();
    new Simpleclock ();
    Application.Run ();
  }

  /**
    Simpleclock constructor, creates a 256x256 clock window and redraws the
    clock every 500ms.
  */
  Simpleclock ()
  {
    win = new Window ("Simpleclock");
    win.SetDefaultSize (256,256);
    win.DeleteEvent += new DeleteEventHandler (OnQuit);
    GLib.Timeout.Add (500, UpdateClock);
    win.Add (this);
    win.ShowAll ();
  }

  /**
    The Draw method draws the clock on the given Cairo.Context in a width by
    height -area.

    Draw clears the canvas, sets up the clock transform and calls the clock
    drawing methods. In the clock transform, the clock area coords go from
    -1.0 to 1.0, rotation 0 is at 12:00 and rotation increases clockwise.
    The clock drawing functions are DrawClockFace, DrawHourHand, DrawMinuteHand,
    DrawSecondHand and DrawPin. The transform origin is at the middle of the
    window and the transform preserves the aspect ratio.

    All operations on the context take place inside a Save-Restore -pair.

    @param cr Cairo.Context to draw on.
    @param width The width of the drawable area.
    @param height The height of the drawable area.
  */
  void Draw (Context cr, uint width, uint height)
  {
    uint boxSize = Math.Min (width, height);

    DateTime date = DateTime.Now;

    cr.Save ();
      // Clear the canvas
      cr.Color = new Color (1, 1, 1);
      cr.Rectangle (0, 0, width, height);
      cr.Fill ();

        // Set canvas transform so that the coords go from -1.0 to 1.0,
        // rotation 0.0 is 12:00 and rotation increases clockwise.
        //
        // First, center the clock box to the window.
      cr.Translate ((width - boxSize) / 2.0, (height - boxSize) / 2.0);
        // Then scale the box so that -1.0 .. 1.0 spans the whole box.
      cr.Scale (boxSize / 2.0, boxSize / 2.0);
        // And move the origin to the center of the box.
      cr.Translate (1.0, 1.0);
        // Finally, rotate CCW by 90 degrees to make rotation 0 point up.
        // We don't need to flip the rotation direction, because angle grows
        // clockwise in the "Y grows down" default transform.
      cr.Rotate (-Math.PI / 2.0);

      // Draw the clock
      DrawClockFace (cr);
      DrawHourHand (cr, (uint)date.Hour);
      DrawMinuteHand (cr, (uint)date.Minute);
      DrawSecondHand (cr, (uint)date.Second);
      DrawPin (cr);
    cr.Restore ();
  }

  /**
    Draws the clock face, a.k.a. the background of the clock.

    @param cr The context to draw on.
  */
  void DrawClockFace (Context cr)
  {
    cr.Save ();
      cr.Color = new Color (0, 0, 0);
      cr.Arc (0.0, 0.0, 0.95, 0.0, 2.0 * Math.PI);
      cr.LineWidth = 0.01;
      cr.Stroke ();
    cr.Restore ();
  }

  /**
    Draws the hour hand of the clock.

    @param cr The context to draw on.
    @param hour The hour as an uint between 0 and 23.
  */
  void DrawHourHand (Context cr, uint hour)
  {
    double rot = (double)(hour % 12) / 12.0;
    cr.Save ();
      cr.Rotate (rot * Math.PI * 2.0);
      cr.Rectangle (0.0, -0.1, 0.6, 0.2);
      cr.Color = new Color (0, 0, 0);
      cr.Fill ();
    cr.Restore ();
  }

  /**
    Draws the minute hand of the clock.

    @param cr The context to draw on.
    @param minute The minute as an uint between 0 and 59.
  */
  void DrawMinuteHand (Context cr, uint minute)
  {
    double rot = (double)minute / 60.0;
    cr.Save ();
      cr.Rotate (rot * Math.PI * 2.0);
      cr.Rectangle (0.0, -0.05, 0.8, 0.1);
      cr.Color = new Color (0, 0, 0);
      cr.Fill ();
    cr.Restore ();
  }

  /**
    Draws the second hand of the clock.

    @param cr The context to draw on.
    @param second The second as an uint between 0 and 59.
  */
  void DrawSecondHand (Context cr, uint second)
  {
    double rot = (double)second / 60.0;
    cr.Save ();
      cr.Rotate (rot * Math.PI * 2.0);
      cr.Rectangle (0.0, -0.025, 0.9, 0.05);
      cr.Color = new Color (0, 0, 0);
      cr.Fill ();
    cr.Restore ();
  }

  /**
    Draws the pin of the clock (just a black circle at origin here.)

    @param cr The context to draw on.
  */
  void DrawPin (Context cr)
  {
    cr.Save ();
      cr.Arc (0.0, 0.0, 0.1, 0.0, 2.0 * Math.PI);
      cr.Color = new Color (0, 0, 0);
      cr.Fill ();
    cr.Restore ();
  }

  /**
    The update method of the clock, called every 500ms by the timeout set in
    the Simpleclock constructor. Queues a draw event for the window and returns true.

    @returns true
  */
  private static bool UpdateClock ()
  {
    win.QueueDraw ();
    return true;
  }

  /**
    The expose event handler for the clock. Gets the Cairo.Context for the
    window and calls Draw with it and the window dimensions.

    @param e The expose event.
    @returns true
  */
  protected override bool OnExposeEvent (Gdk.EventExpose e)
  {
    using ( Context cr = Gdk.CairoHelper.Create (e.Window) )
    {
      int w, h;
      e.Window.GetSize (out w, out h);
      Draw (cr, (uint)w, (uint)h);
    }
    return true;
  }

  /**
    The quit event handler. Calls Application.Quit.
  */
  void OnQuit (object sender, DeleteEventArgs e)
  {
    Application.Quit ();
  }
}
