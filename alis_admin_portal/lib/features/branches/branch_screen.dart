import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class BranchScreen extends StatelessWidget {
  const BranchScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Branches',
      selectedIndex: 4,
      child: Center(
        child: Text(
          'Branches Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
