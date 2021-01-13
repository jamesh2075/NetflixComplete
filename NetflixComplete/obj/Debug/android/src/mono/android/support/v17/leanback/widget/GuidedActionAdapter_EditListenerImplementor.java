package mono.android.support.v17.leanback.widget;


public class GuidedActionAdapter_EditListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.v17.leanback.widget.GuidedActionAdapter.EditListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGuidedActionEdited:(Landroid/support/v17/leanback/widget/GuidedAction;)V:GetOnGuidedActionEdited_Landroid_support_v17_leanback_widget_GuidedAction_Handler:Android.Support.V17.Leanback.Widget.GuidedActionAdapter/IEditListenerInvoker, Xamarin.Android.Support.v17.Leanback\n" +
			"n_onGuidedActionEditedAndProceed:(Landroid/support/v17/leanback/widget/GuidedAction;)J:GetOnGuidedActionEditedAndProceed_Landroid_support_v17_leanback_widget_GuidedAction_Handler:Android.Support.V17.Leanback.Widget.GuidedActionAdapter/IEditListenerInvoker, Xamarin.Android.Support.v17.Leanback\n" +
			"n_onImeClose:()V:GetOnImeCloseHandler:Android.Support.V17.Leanback.Widget.GuidedActionAdapter/IEditListenerInvoker, Xamarin.Android.Support.v17.Leanback\n" +
			"n_onImeOpen:()V:GetOnImeOpenHandler:Android.Support.V17.Leanback.Widget.GuidedActionAdapter/IEditListenerInvoker, Xamarin.Android.Support.v17.Leanback\n" +
			"";
		mono.android.Runtime.register ("Android.Support.V17.Leanback.Widget.GuidedActionAdapter+IEditListenerImplementor, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GuidedActionAdapter_EditListenerImplementor.class, __md_methods);
	}


	public GuidedActionAdapter_EditListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == GuidedActionAdapter_EditListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.V17.Leanback.Widget.GuidedActionAdapter+IEditListenerImplementor, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onGuidedActionEdited (android.support.v17.leanback.widget.GuidedAction p0)
	{
		n_onGuidedActionEdited (p0);
	}

	private native void n_onGuidedActionEdited (android.support.v17.leanback.widget.GuidedAction p0);


	public long onGuidedActionEditedAndProceed (android.support.v17.leanback.widget.GuidedAction p0)
	{
		return n_onGuidedActionEditedAndProceed (p0);
	}

	private native long n_onGuidedActionEditedAndProceed (android.support.v17.leanback.widget.GuidedAction p0);


	public void onImeClose ()
	{
		n_onImeClose ();
	}

	private native void n_onImeClose ();


	public void onImeOpen ()
	{
		n_onImeOpen ();
	}

	private native void n_onImeOpen ();

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
