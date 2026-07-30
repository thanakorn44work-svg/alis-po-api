import 'package:flutter/material.dart';

import '../../../core/constants/app_colors.dart';
import '../../../core/widgets/app_card.dart';

class BranchSummaryCard extends StatelessWidget {
  const BranchSummaryCard({
    super.key,
    required this.branchName,
    required this.orders,
  });

  final String branchName;
  final int orders;

  @override
  Widget build(BuildContext context) {
    return AppCard(
      padding: const EdgeInsets.all(18),
      child: Row(
        children: [
          const CircleAvatar(
            radius: 22,
            backgroundColor: AppColors.orange,
            child: Icon(Icons.store, color: Colors.white),
          ),

          const SizedBox(width: 16),

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  branchName,
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),

                const SizedBox(height: 4),

                Text(
                  "$orders Orders",
                  style: const TextStyle(color: AppColors.subText),
                ),
              ],
            ),
          ),

          const Icon(Icons.chevron_right, color: AppColors.subText),
        ],
      ),
    );
  }
}
