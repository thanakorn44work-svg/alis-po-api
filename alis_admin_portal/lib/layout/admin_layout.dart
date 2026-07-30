import 'package:flutter/material.dart';

import '../core/constants/app_colors.dart';
import '../core/constants/app_spacing.dart';
import '../core/widgets/app_sidebar.dart';
import '../core/widgets/app_topbar.dart';

class AdminLayout extends StatefulWidget {
  const AdminLayout({
    super.key,
    required this.title,
    required this.selectedIndex,
    required this.child,
  });

  final String title;
  final int selectedIndex;
  final Widget child;

  @override
  State<AdminLayout> createState() => _AdminLayoutState();
}

class _AdminLayoutState extends State<AdminLayout> {
  late int _selectedIndex;

  @override
  void initState() {
    super.initState();
    _selectedIndex = widget.selectedIndex;
  }

  void _onMenuSelected(int index) {
    if (index == _selectedIndex) return;

    setState(() {
      _selectedIndex = index;
    });

    switch (index) {
      case 0:
        Navigator.pushReplacementNamed(context, '/dashboard');
        break;

      case 1:
        Navigator.pushReplacementNamed(context, '/products');
        break;

      case 2:
        Navigator.pushReplacementNamed(context, '/categories');
        break;

      case 3:
        Navigator.pushReplacementNamed(context, '/purchase-orders');
        break;

      case 4:
        Navigator.pushReplacementNamed(context, '/branches');
        break;

      case 5:
        Navigator.pushReplacementNamed(context, '/users');
        break;

      case 6:
        Navigator.pushReplacementNamed(context, '/reports');
        break;

      case 7:
        Navigator.pushReplacementNamed(context, '/settings');
        break;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: Row(
        children: [
          AppSidebar(
            selectedIndex: _selectedIndex,
            onSelected: _onMenuSelected,
          ),

          Expanded(
            child: Column(
              children: [
                AppTopbar(title: widget.title),

                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.all(AppSpacing.lg),
                    child: widget.child,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
