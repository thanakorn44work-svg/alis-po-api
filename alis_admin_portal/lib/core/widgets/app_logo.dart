import 'package:flutter/material.dart';

import '../constants/app_colors.dart';

class AppLogo extends StatelessWidget {
  const AppLogo({super.key, this.size = 70, this.showText = true});

  final double size;
  final bool showText;

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: size,
          height: size,
          decoration: const BoxDecoration(
            color: AppColors.orange,
            shape: BoxShape.circle,
          ),
          child: Icon(Icons.restaurant, color: Colors.white, size: size * .5),
        ),

        if (showText) ...[
          const SizedBox(height: 14),

          const Text(
            "ALI'S",
            style: TextStyle(
              color: Colors.white,
              fontSize: 24,
              fontWeight: FontWeight.bold,
              letterSpacing: 1,
            ),
          ),

          const SizedBox(height: 4),

          const Text(
            "Admin Portal",
            style: TextStyle(color: Colors.white70, fontSize: 13),
          ),
        ],
      ],
    );
  }
}
