import 'package:flutter/material.dart';

import '../constants/app_colors.dart';
import '../constants/app_radius.dart';
import '../constants/app_shadow.dart';

class AppCard extends StatelessWidget {
  const AppCard({
    super.key,
    required this.child,
    this.padding = const EdgeInsets.all(24),
    this.margin = EdgeInsets.zero,
    this.width,
    this.height,
    this.onTap,
  });

  final Widget child;
  final EdgeInsets padding;
  final EdgeInsets margin;
  final double? width;
  final double? height;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    Widget card = AnimatedContainer(
      duration: const Duration(milliseconds: 180),
      curve: Curves.easeInOut,

      width: width,
      height: height,

      margin: margin,
      padding: padding,

      decoration: BoxDecoration(
        color: AppColors.card,

        borderRadius: BorderRadius.circular(AppRadius.xl),

        border: Border.all(color: AppColors.border),

        boxShadow: AppShadow.card,
      ),

      child: child,
    );

    if (onTap != null) {
      card = InkWell(
        borderRadius: BorderRadius.circular(AppRadius.xl),
        onTap: onTap,
        child: card,
      );
    }

    return card;
  }
}
