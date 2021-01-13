package mono.android.support.v17.leanback.widget;


public class GuidedActionAdapter_FocusListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.v17.leanback.widget.GuidedActionAdapter.FocusListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGuidedActionFocused:(Landroid/support/v17/leanback/widget/GuidedAction;)V:GetOnGuidedActionFocused_Landroid_support_v17_leanback_widget_GuidedAction_Handler:Android.Support.V17.Leanback.Widget.GuidedActionAdapter/IFocusListenerInvoker, Xamarin.Android.Support.v17.Leanback\n" +
			"";
		mono.android.Runtime.register ("Android.Support.V17.Leanback.Widget.GuidedActionAdapter+IFocusListenerImplementor, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GuidedActionAdapter_FocusListenerImplementor.class, __md_methods);
	}


	public GuidedActionAdapter_FocusListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == GuidedActionAdapter_FocusListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.V17.Leanback.Widget.GuidedActionAdapter+IFocusListenerImplementor, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onGuidedActionFocused (android.support.v17.leanback.widget.GuidedAction p0)
	{
		n_onGuidedActionFocused (p0);
	}

	private native void n_onGuidedActionFocused (android.support.v17.leanback.widget.GuidedAction p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
