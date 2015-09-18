//package com.viewpagerindicator.sample;

//import android.support.v4.app.Fragment;
//import android.support.v4.app.FragmentManager;
//import android.support.v4.app.FragmentPagerAdapter;
//import com.viewpagerindicator.IconPagerAdapter;


using Android.Support.V4.App;
using Com.ViewPagerIndicator;
using Java.Lang;
using System;
using R = ViewPagerIndicator.Net.Sample.Resource;


namespace ViewPagerIndicator.Net.Sample
{
    public class TestFragmentAdapter : FragmentPagerAdapter, IIconPagerAdapter
    {
        protected static string[] CONTENT = new string[] { "This", "Is", "A", "Test", };
        protected static int[] ICONS = new int[] {
            R.Drawable.perm_group_calendar,
            R.Drawable.perm_group_camera,
            R.Drawable.perm_group_device_alarms,
            R.Drawable.perm_group_location
    };

        private int mCount = CONTENT.Length;

        public TestFragmentAdapter(FragmentManager fm)
            : base(fm)
        {

        }

        public override Fragment GetItem(int position)
        {
            return TestFragment.newInstance(CONTENT[position % CONTENT.Length]);
            //throw new NotImplementedException();
        }


        //@Override
        //public int getCount() {
        //    return mCount;
        //}

        public override int Count
        {
            get { return mCount; }
        }

        public int getCount()
        {
            return mCount;
        }
        
        //@Override
        public new string GetPageTitle(int position)
        {
            return CONTENT[position % CONTENT.Length];
        }
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(CONTENT[position % CONTENT.Length].ToUpper());
        }
        //@Override
        public int getIconResId(int index)
        {
            return ICONS[index % ICONS.Length];
        }

        public void setCount(int count)
        {
            if (count > 0 && count <= 10)
            {
                mCount = count;
                NotifyDataSetChanged();
            }
        }
    }
}