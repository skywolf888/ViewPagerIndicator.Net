/*
 * Copyright (C) 2011 The Android Open Source Project
 * Copyright (C) 2011 Jake Wharton
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//package com.viewpagerindicator;

//import android.content.Context;
//import android.support.v4.view.PagerAdapter;
//import android.support.v4.view.ViewPager;
//import android.support.v4.view.ViewPager.OnPageChangeListener;
//import android.util.AttributeSet;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.HorizontalScrollView;
//import android.widget.LinearLayout;
//import android.widget.TextView;

using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
//import static android.view.ViewGroup.LayoutParams.MATCH_PARENT;
//import static android.view.ViewGroup.LayoutParams.WRAP_CONTENT;
using Android.Widget;
using Java.Lang;
using R=ViewPagerIndicator.Net.Resource;

namespace Com.ViewPagerIndicator
{
    /**
     * This widget implements the dynamic action bar tab behavior that can change
     * across different configurations or circumstances.
     */
    public class TabPageIndicator : HorizontalScrollView, IPageIndicator
    {
        /** Title text used when no title is provided by the adapter. */
        private const string EMPTY_TITLE = "";

        /**
         * Interface for a callback when the selected tab has been reselected.
         */
        public interface OnTabReselectedListener
        {
            /**
             * Callback when the selected tab has been reselected.
             *
             * @param position Position of the current center item.
             */
            void onTabReselected(int position);
        }

        private IRunnable mTabSelector;

        //private  OnClickListener mTabClickListener = new OnClickListener() {
        //    public void onClick(View view) {
        //        TabView tabView = (TabView)view;
        //         int oldSelected = mViewPager.getCurrentItem();
        //         int newSelected = tabView.getIndex();
        //        mViewPager.setCurrentItem(newSelected);
        //        if (oldSelected == newSelected && mTabReselectedListener != null) {
        //            mTabReselectedListener.onTabReselected(newSelected);
        //        }
        //    }
        //};
        private IOnClickListener mTabClickListener;


        class TabClicker : Java.Lang.Object, IOnClickListener
        {
            private TabPageIndicator minst;

            public TabClicker(TabPageIndicator inst)
            {
                minst = inst;
            }
            public void OnClick(View v)
            {
                TabView tabView = (TabView)v;
                int oldSelected = minst.mViewPager.CurrentItem;
                int newSelected = tabView.getIndex();
                minst.mViewPager.SetCurrentItem(newSelected, true);
                if (oldSelected == newSelected && minst.mTabReselectedListener != null)
                {
                    minst.mTabReselectedListener.onTabReselected(newSelected);
                }
            }
        }

        private IcsLinearLayout mTabLayout;

        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mListener;

        private int mMaxTabWidth;
        private int mSelectedTabIndex;

        private OnTabReselectedListener mTabReselectedListener;

        public TabPageIndicator(Context context)
            : this(context, null)
        {

        }

        public TabPageIndicator(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            mTabClickListener = new TabClicker(this);
            HorizontalScrollBarEnabled = false;

            mTabLayout = new IcsLinearLayout(context, R.Attribute.vpiTabPageIndicatorStyle);
            AddView(mTabLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent));
        }

        public void setOnTabReselectedListener(OnTabReselectedListener listener)
        {
            mTabReselectedListener = listener;
        }

        //@Override
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            bool lockedExpanded = widthMode == MeasureSpecMode.Exactly;
            //setFillViewport(lockedExpanded);
            FillViewport = lockedExpanded;
            int childCount = mTabLayout.ChildCount;
            if (childCount > 1 && (widthMode == MeasureSpecMode.Exactly || widthMode == MeasureSpecMode.AtMost))
            {
                if (childCount > 2)
                {
                    mMaxTabWidth = (int)(MeasureSpec.GetSize(widthMeasureSpec) * 0.4f);
                }
                else
                {
                    mMaxTabWidth = MeasureSpec.GetSize(widthMeasureSpec) / 2;
                }
            }
            else
            {
                mMaxTabWidth = -1;
            }

            int oldWidth = MeasuredWidth;
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int newWidth = MeasuredWidth;

            if (lockedExpanded && oldWidth != newWidth)
            {
                // Recenter the tab display if we're at a new (scrollable) size.
                setCurrentItem(mSelectedTabIndex);
            }
        }

        class AnimateToTab : Java.Lang.Object, IRunnable
        {
            private View tabView;
            private TabPageIndicator tabPageIndicator;

            public AnimateToTab(View tabView, TabPageIndicator tabPageIndicator)
            {
                // TODO: Complete member initialization
                this.tabView = tabView;
                this.tabPageIndicator = tabPageIndicator;
            }

            public void Run()
            {
                int scrollPos = tabView.Left - (tabPageIndicator.Width - tabView.Width) / 2;
                tabPageIndicator.SmoothScrollTo(scrollPos, 0);
                tabPageIndicator.mTabSelector = null;
            }
        }

        private void animateToTab(int position)
        {
            View tabView = mTabLayout.GetChildAt(position);
            if (mTabSelector != null)
            {
                RemoveCallbacks(mTabSelector);
            }
            //throw new Exception("");
            //mTabSelector = new Runnable() {
            //    public void run() {
            //         int scrollPos = tabView.getLeft() - (getWidth() - tabView.getWidth()) / 2;
            //        smoothScrollTo(scrollPos, 0);
            //        mTabSelector = null;
            //    }
            //};

            mTabSelector = new AnimateToTab(tabView, this);
            Post(mTabSelector);
        }

        //@Override
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            if (mTabSelector != null)
            {
                // Re-post the selector we saved
                Post(mTabSelector);
            }
        }

        //@Override
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            if (mTabSelector != null)
            {
                RemoveCallbacks(mTabSelector);
            }
        }

        private void addTab(int index, string text, int iconResId)
        {
            TabView tabView = new TabView(this.Context, this.mMaxTabWidth);

            tabView.mIndex = index;
            tabView.Focusable = true;
            tabView.SetOnClickListener(mTabClickListener);

            tabView.Text = text;

            if (iconResId != 0)
            {
                tabView.SetCompoundDrawablesWithIntrinsicBounds(iconResId, 0, 0, 0);
            }

            mTabLayout.AddView(tabView, new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 1));
        }

        //@Override
        public void OnPageScrollStateChanged(int arg0)
        {
            if (mListener != null)
            {
                mListener.OnPageScrollStateChanged(arg0);
            }
        }

        //@Override
        public void OnPageScrolled(int arg0, float arg1, int arg2)
        {
            if (mListener != null)
            {
                mListener.OnPageScrolled(arg0, arg1, arg2);
            }
        }

        //@Override
        public void OnPageSelected(int arg0)
        {
            setCurrentItem(arg0);
            if (mListener != null)
            {
                mListener.OnPageSelected(arg0);
            }
        }

        //@Override
        public void setViewPager(ViewPager view)
        {
            if (mViewPager == view)
            {
                return;
            }
            if (mViewPager != null)
            {
                mViewPager.SetOnPageChangeListener(null);
            }
            PagerAdapter adapter = view.Adapter;
            if (adapter == null)
            {
                throw new IllegalStateException("ViewPager does not have adapter instance.");
            }
            mViewPager = view;
            view.SetOnPageChangeListener(this);
            notifyDataSetChanged();
        }

        public void notifyDataSetChanged()
        {
            mTabLayout.RemoveAllViews();
            PagerAdapter adapter = mViewPager.Adapter;
            IIconPagerAdapter iconAdapter = null;
            if (adapter is IIconPagerAdapter)
            {
                iconAdapter = (IIconPagerAdapter)adapter;
            }
            int count = adapter.Count;
            for (int i = 0; i < count; i++)
            {
                string title =adapter.GetPageTitle(i);
                if (title == null)
                {
                    title = EMPTY_TITLE;
                }
                int iconResId = 0;
                if (iconAdapter != null)
                {
                    iconResId = iconAdapter.getIconResId(i);
                }
                addTab(i, title, iconResId);
            }
            if (mSelectedTabIndex > count)
            {
                mSelectedTabIndex = count - 1;
            }
            setCurrentItem(mSelectedTabIndex);
            RequestLayout();
        }

        //@Override
        public void setViewPager(ViewPager view, int initialPosition)
        {
            setViewPager(view);
            setCurrentItem(initialPosition);
        }

        //@Override
        public void setCurrentItem(int item)
        {
            if (mViewPager == null)
            {
                throw new IllegalStateException("ViewPager has not been bound.");
            }
            mSelectedTabIndex = item;
            mViewPager.SetCurrentItem(item, true);

            int tabCount = mTabLayout.ChildCount;
            for (int i = 0; i < tabCount; i++)
            {
                View child = mTabLayout.GetChildAt(i);
                bool isSelected = (i == item);
                child.Selected = isSelected;
                if (isSelected)
                {
                    animateToTab(item);
                }
            }
        }

        //@Override
        public void setOnPageChangeListener(ViewPager.IOnPageChangeListener listener)
        {
            mListener = listener;
        }

        private class TabView : TextView
        {
            public int mIndex;
            private int mMaxTabWidth;
            public TabView(Context context, int maxtabwidth)
                : base(context, null, R.Attribute.vpiTabPageIndicatorStyle)
            {
                mMaxTabWidth = maxtabwidth;
            }

            //@Override
            protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

                // Re-measure if we went beyond our maximum size.
                if (mMaxTabWidth > 0 && MeasuredWidth > mMaxTabWidth)
                {
                    base.OnMeasure(MeasureSpec.MakeMeasureSpec(mMaxTabWidth, MeasureSpecMode.Exactly),
                            heightMeasureSpec);
                }
            }

            public int getIndex()
            {
                return mIndex;
            }
        }


    }
}