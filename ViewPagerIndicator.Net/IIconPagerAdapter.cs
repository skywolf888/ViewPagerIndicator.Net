//package com.viewpagerindicator;
namespace Com.ViewPagerIndicator
{
    public interface IIconPagerAdapter
    {
        /**
         * Get icon representing the page at {@code index} in the adapter.
         */
        int getIconResId(int index);

        // From PagerAdapter
        int getCount();
    }
}