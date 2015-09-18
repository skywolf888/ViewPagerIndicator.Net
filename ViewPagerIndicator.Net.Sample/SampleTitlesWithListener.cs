//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import android.widget.Toast;

//import com.viewpagerindicator.TitlePageIndicator;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using Com.ViewPagerIndicator;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleTitlesWithListener")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleTitlesWithListener : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_titles);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            mIndicator = (TitlePageIndicator)FindViewById(R.Id.indicator);
            mIndicator.setViewPager(mPager);
            mIndicator.setOnPageChangeListener(new PageChangeListener(this));
            //We set this on the indicator, NOT the pager
            //mIndicator.setOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            //    @Override
            //    public void onPageSelected(int position) {
            //        Toast.makeText(SampleTitlesWithListener.this, "Changed to page " + position, Toast.LENGTH_SHORT).show();
            //    }

            //    @Override
            //    public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
            //    }

            //    @Override
            //    public void onPageScrollStateChanged(int state) {
            //    }
            //});
        }

        class PageChangeListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
        {
            private Context mContext;

            public PageChangeListener(Context context)
            {
                mContext = context;
            }

            public void OnPageScrollStateChanged(int state)
            {
                //throw new System.NotImplementedException();
            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                //throw new System.NotImplementedException();
            }

            public void OnPageSelected(int position)
            {
                Toast.MakeText(mContext, "Changed to page " + position.ToString(), ToastLength.Short).Show();
            }
        }
    }
}