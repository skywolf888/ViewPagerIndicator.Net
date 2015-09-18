//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;

//import com.viewpagerindicator.TitlePageIndicator;
//import com.viewpagerindicator.TitlePageIndicator.IndicatorStyle;
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
    [Activity(Label = "SampleTitlesStyledMethods")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleTitlesStyledMethods : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_titles);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            TitlePageIndicator indicator = (TitlePageIndicator)FindViewById(R.Id.indicator);
            mIndicator = indicator;
            indicator.setViewPager(mPager);

            float density = Resources.DisplayMetrics.Density;
            indicator.SetBackgroundColor(new Color(0x18FF0000));
            indicator.setFooterColor(new Color(0xAA2222));
            indicator.setFooterLineHeight(1 * density); //1dp
            indicator.setFooterIndicatorHeight(3 * density); //3dp
            indicator.setFooterIndicatorStyle(IndicatorStyle.Underline);
            indicator.setTextColor(new Color(0x000000));
            indicator.setSelectedColor(new Color(0x000000));
            indicator.setSelectedBold(true);
        }
    }
}