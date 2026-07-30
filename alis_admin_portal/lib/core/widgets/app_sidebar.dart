import 'package:flutter/material.dart';

import '../constants/app_colors.dart';

class AppSidebar extends StatelessWidget {
  const AppSidebar({
    super.key,
    required this.selectedIndex,
    required this.onSelected,
  });

  final int selectedIndex;
  final ValueChanged<int> onSelected;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 260,
      color: AppColors.sidebarBackground,
      child: SafeArea(
        child: Column(
          children: [
            const SizedBox(height: 30),

            const CircleAvatar(
              radius: 34,
              backgroundColor: AppColors.orange,
              child: Icon(
                Icons.admin_panel_settings_rounded,
                color: Colors.white,
                size: 34,
              ),
            ),

            const SizedBox(height: 18),

            const Text(
              "ALI'S",
              style: TextStyle(
                color: Colors.white,
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),

            const Text("Admin Portal", style: TextStyle(color: Colors.white70)),

            const SizedBox(height: 35),

            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                children: [
                  _item(
                    index: 0,
                    icon: Icons.dashboard_rounded,
                    title: "Dashboard",
                  ),
                  _item(
                    index: 1,
                    icon: Icons.inventory_2_rounded,
                    title: "Products",
                  ),
                  _item(
                    index: 2,
                    icon: Icons.category_rounded,
                    title: "Categories",
                  ),
                  _item(
                    index: 3,
                    icon: Icons.receipt_long_rounded,
                    title: "Purchase Orders",
                  ),
                  _item(index: 4, icon: Icons.store_rounded, title: "Branches"),
                  _item(
                    index: 5,
                    icon: Icons.people_alt_rounded,
                    title: "Users",
                  ),
                  _item(
                    index: 6,
                    icon: Icons.bar_chart_rounded,
                    title: "Reports",
                  ),
                  _item(
                    index: 7,
                    icon: Icons.settings_rounded,
                    title: "Settings",
                  ),
                ],
              ),
            ),

            const Divider(color: Colors.white24, height: 1),

            const Padding(
              padding: EdgeInsets.all(20),
              child: Text(
                "Version 1.0.0",
                style: TextStyle(color: Colors.white54, fontSize: 12),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _item({
    required int index,
    required IconData icon,
    required String title,
  }) {
    final selected = index == selectedIndex;

    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: () => onSelected(index),
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 180),
          padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 15),
          decoration: BoxDecoration(
            color: selected ? AppColors.orange : Colors.transparent,
            borderRadius: BorderRadius.circular(14),
          ),
          child: Row(
            children: [
              Icon(icon, color: Colors.white),
              const SizedBox(width: 16),
              Expanded(
                child: Text(
                  title,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
