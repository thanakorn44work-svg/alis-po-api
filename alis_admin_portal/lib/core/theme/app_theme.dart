import 'package:flutter/material.dart';

import '../constants/app_colors.dart';
import '../constants/app_radius.dart';
import '../constants/app_shadow.dart';

class AppTheme {
  AppTheme._();

  static ThemeData get lightTheme => light;
  static ThemeData get light {
    return ThemeData(
      useMaterial3: true,

      fontFamily: 'Roboto',

      scaffoldBackgroundColor: AppColors.background,

      colorScheme: ColorScheme.fromSeed(
        seedColor: AppColors.orange,
        primary: AppColors.orange,
        brightness: Brightness.light,
      ),

      //------------------------------------------------------
      // AppBar
      //------------------------------------------------------
      appBarTheme: const AppBarTheme(
        elevation: 0,
        centerTitle: false,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.text,
      ),

      //------------------------------------------------------
      // Card
      //------------------------------------------------------
      cardTheme: CardThemeData(
        color: AppColors.card,
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.xl),
        ),
      ),

      //------------------------------------------------------
      // Input
      //------------------------------------------------------
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: Colors.white,

        contentPadding: const EdgeInsets.symmetric(
          horizontal: 18,
          vertical: 18,
        ),

        hintStyle: const TextStyle(color: AppColors.hint),

        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.lg),
          borderSide: const BorderSide(color: AppColors.border),
        ),

        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.lg),
          borderSide: const BorderSide(color: AppColors.border),
        ),

        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.lg),
          borderSide: const BorderSide(color: AppColors.orange, width: 2),
        ),
      ),

      //------------------------------------------------------
      // Button
      //------------------------------------------------------
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: AppColors.orange,
          foregroundColor: Colors.white,

          minimumSize: const Size(double.infinity, 54),

          elevation: 0,

          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.lg),
          ),
        ),
      ),

      //------------------------------------------------------
      // Divider
      //------------------------------------------------------
      dividerColor: AppColors.divider,

      //------------------------------------------------------
      // Text
      //------------------------------------------------------
      textTheme: const TextTheme(
        headlineLarge: TextStyle(
          fontSize: 38,
          fontWeight: FontWeight.bold,
          color: AppColors.text,
        ),

        headlineMedium: TextStyle(
          fontSize: 30,
          fontWeight: FontWeight.bold,
          color: AppColors.text,
        ),

        titleLarge: TextStyle(
          fontSize: 22,
          fontWeight: FontWeight.bold,
          color: AppColors.text,
        ),

        titleMedium: TextStyle(
          fontSize: 18,
          fontWeight: FontWeight.w600,
          color: AppColors.text,
        ),

        bodyLarge: TextStyle(fontSize: 16, color: AppColors.text),

        bodyMedium: TextStyle(fontSize: 14, color: AppColors.subText),

        bodySmall: TextStyle(fontSize: 12, color: AppColors.subText),
      ),

      //------------------------------------------------------
      // Splash
      //------------------------------------------------------
      splashColor: Colors.transparent,

      highlightColor: Colors.transparent,

      shadowColor: AppShadow.card.first.color,
    );
  }
}
