//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.app.Fragment;
//import android.view.Gravity;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.LinearLayout;
//import android.widget.LinearLayout.LayoutParams;
//import android.widget.TextView;


using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System.Text;


namespace ViewPagerIndicator.Net.Sample
{
    public sealed class TestFragment : Fragment
    {
        private const string KEY_CONTENT = "TestFragment:Content";

        public static TestFragment newInstance(string content)
        {
            TestFragment fragment = new TestFragment();

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 20; i++)
            {
                builder.Append(content).Append(" ");
            }
            builder.Remove(builder.Length - 1, 1);
            fragment.mContent = builder.ToString();

            return fragment;
        }

        private string mContent = "???";

        //@Override
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((savedInstanceState != null) && savedInstanceState.ContainsKey(KEY_CONTENT))
            {
                mContent = savedInstanceState.GetString(KEY_CONTENT);
            }
        }

        //@Override
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            TextView text = new TextView(this.Activity);
            text.Gravity = GravityFlags.Center;
            text.Text = mContent;
            text.TextSize = 20 * this.Resources.DisplayMetrics.Density;
            text.SetPadding(20, 20, 20, 20);

            LinearLayout layout = new LinearLayout(this.Activity);
            layout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            layout.SetGravity(GravityFlags.Center);
            layout.AddView(text);

            return layout;
        }

        //@Override
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KEY_CONTENT, mContent);
        }
    }
}