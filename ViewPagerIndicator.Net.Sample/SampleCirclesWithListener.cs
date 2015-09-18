//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import android.widget.Toast;
//import com.viewpagerindicator.CirclePageIndicator;


using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Widget;
using Com.ViewPagerIndicator;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleCirclesWithListener")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]


    public class SampleCirclesWithListener : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_circles);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            mIndicator = (CirclePageIndicator)FindViewById(R.Id.indicator);
            mIndicator.setViewPager(mPager);

            //We set this on the indicator, NOT the pager
            //mIndicator.setOnPageChangeListener(new ViewPager.IOnPageChangeListener() {
            //    @Override
            //    public void onPageSelected(int position) {
            //        Toast.makeText(SampleCirclesWithListener.this, "Changed to page " + position, Toast.LENGTH_SHORT).show();
            //    }

            //    @Override
            //    public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
            //    }

            //    @Override
            //    public void onPageScrollStateChanged(int state) {
            //    }
            //});

            mIndicator.setOnPageChangeListener(new PageListener(this));
        }

        class PageListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
        {
            private SampleCirclesWithListener sampleCirclesWithListener;

            public PageListener(SampleCirclesWithListener sampleCirclesWithListener)
            {
                // TODO: Complete member initialization
                this.sampleCirclesWithListener = sampleCirclesWithListener;
            }
            //@Override
            public void OnPageSelected(int position)
            {
                Toast.MakeText(sampleCirclesWithListener, "Changed to page " + position, ToastLength.Short).Show();
            }

            //@Override
            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
            }

            //@Override
            public void OnPageScrollStateChanged(int state)
            {
            }
        }
    }
}