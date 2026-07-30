import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class ProductScreen extends StatelessWidget {
  const ProductScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Products',
      selectedIndex: 1,
      child: Center(
        child: Text(
          'Products Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
