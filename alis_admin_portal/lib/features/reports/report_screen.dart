import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class ReportScreen extends StatelessWidget {
  const ReportScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Reports',
      selectedIndex: 6,
      child: Center(
        child: Text(
          'Reports Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
