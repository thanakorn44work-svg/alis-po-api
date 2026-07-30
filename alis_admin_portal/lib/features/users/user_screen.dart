import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class UserScreen extends StatelessWidget {
  const UserScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Users',
      selectedIndex: 5,
      child: Center(
        child: Text(
          'Users Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
