//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import com.viewpagerindicator.CirclePageIndicator;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Com.ViewPagerIndicator;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleCirclesStyledMethods")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleCirclesStyledMethods : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_circles);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            CirclePageIndicator indicator = (CirclePageIndicator)FindViewById(R.Id.indicator);
            mIndicator = indicator;
            indicator.setViewPager(mPager);

            float density = this.Resources.DisplayMetrics.Density;

            indicator.SetBackgroundColor(new Color(0xCCCCCC));
            indicator.setRadius(10 * density);

            indicator.setPageColor(new Color(0x0000FF));
            indicator.setFillColor(new Color(0x888888));
            indicator.setStrokeColor(new Color(0x000000));
            indicator.setStrokeWidth(2 * density);
        }
    }
}