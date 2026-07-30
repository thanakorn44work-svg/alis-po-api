import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class SettingScreen extends StatelessWidget {
  const SettingScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Settings',
      selectedIndex: 7,
      child: Center(
        child: Text(
          'Settings Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
