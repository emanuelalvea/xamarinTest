package md5db1d84f0b2adc09d2fa403805cf2e153;


public class MainActivity_OnDismissListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.content.DialogInterface.OnDismissListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDismiss:(Landroid/content/DialogInterface;)V:GetOnDismiss_Landroid_content_DialogInterface_Handler:Android.Content.IDialogInterfaceOnDismissListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("App3.MainActivity+OnDismissListener, App3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MainActivity_OnDismissListener.class, __md_methods);
	}


	public MainActivity_OnDismissListener () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainActivity_OnDismissListener.class)
			mono.android.TypeManager.Activate ("App3.MainActivity+OnDismissListener, App3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onDismiss (android.content.DialogInterface p0)
	{
		n_onDismiss (p0);
	}

	private native void n_onDismiss (android.content.DialogInterface p0);

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
