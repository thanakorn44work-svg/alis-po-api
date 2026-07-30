import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class CategoryScreen extends StatelessWidget {
  const CategoryScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Categories',
      selectedIndex: 2,
      child: Center(
        child: Text(
          'Categories Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
