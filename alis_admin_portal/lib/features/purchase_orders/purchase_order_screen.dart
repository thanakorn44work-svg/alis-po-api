import 'package:flutter/material.dart';

import '../../layout/admin_layout.dart';

class PurchaseOrderScreen extends StatelessWidget {
  const PurchaseOrderScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return const AdminLayout(
      title: 'Purchase Orders',
      selectedIndex: 3,
      child: Center(
        child: Text(
          'Purchase Orders Screen',
          style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
