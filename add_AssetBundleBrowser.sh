#!/bin/sh
# Unityプロジェクトに AssetBundles-Browser を入れる.
# https://github.com/Unity-Technologies/AssetBundles-Browser
# Unity Editor v5.6 以降.

UNITYROOTDIR=`pwd`

/bin/rm -rf AssetBundles-Browser
/bin/rm -rf ${UNITYROOTDIR}/Assets/AssetBundles-Browser
git clone git@github.com:Unity-Technologies/AssetBundles-Browser.git
/bin/cp -fvR AssetBundles-Browser ${UNITYROOTDIR}/Assets
/bin/rm -rf AssetBundles-Browser
