//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import com.viewpagerindicator.LinePageIndicator;


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
    [Activity(Label = "SampleLinesStyledMethods")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleLinesStyledMethods : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_lines);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = (ViewPager)FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            LinePageIndicator indicator = (LinePageIndicator)FindViewById(R.Id.indicator);
            mIndicator = indicator;
            indicator.setViewPager(mPager);

            float density = Resources.DisplayMetrics.Density;
            indicator.setSelectedColor(new Color(0xFF0000));
            indicator.setUnselectedColor(new Color(0x888888));
            indicator.setStrokeWidth(4 * density);
            indicator.setLineWidth(30 * density);
        }
    }
}