
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using QHSalesApp;
using QHSalesApp.Droid;


[assembly:ExportRenderer(typeof(SelectAllEntry), typeof(SelectAllEntryRenderer))]
namespace QHSalesApp.Droid
{
     class SelectAllEntryRenderer :EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if(e.OldElement==null)
            {
                var nativeEditTex = (global::Android.Widget.EditText)Control;
                nativeEditTex.SetSelectAllOnFocus(true);
            }
            //if (Control != null)
            //{
            //    Control.SetBackgroundColor(global::Android.Graphics.Color.LightGreen);
            //}
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        //{
        //    base.OnElementChanged(e);
        //    var nativeTextField = (UITextField)Control;
        //    nativeTextField.EditingDidBegin += (object sender, EventArgs eIos) => {
        //        nativeTextField.PerformSelector(new Selector("selectAll"), null, 0.0f);
        //    };
        //}
    }
}