import 'package:flutter/material.dart';

import '../../core/constants/app_colors.dart';
import '../../core/widgets/app_card.dart';
import '../../layout/admin_layout.dart';
import 'widgets/dashboard_stat_card.dart';

class DashboardScreen extends StatelessWidget {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return AdminLayout(
      title: 'Dashboard',
      selectedIndex: 0,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(30),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Welcome back, Administrator 👋',
              style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
            ),

            const SizedBox(height: 8),

            const Text(
              'Overview of your restaurant operations.',
              style: TextStyle(color: AppColors.subText),
            ),

            const SizedBox(height: 30),

            GridView.count(
              crossAxisCount: 4,
              crossAxisSpacing: 20,
              mainAxisSpacing: 20,
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              childAspectRatio: 2.2,
              children: const [
                DashboardStatCard(
                  title: "Today's Orders",
                  value: "126",
                  subtitle: "+12% from yesterday",
                  icon: Icons.receipt_long,
                  color: AppColors.orders,
                ),
                DashboardStatCard(
                  title: "Products",
                  value: "482",
                  subtitle: "Available Items",
                  icon: Icons.inventory_2,
                  color: AppColors.products,
                ),
                DashboardStatCard(
                  title: "Branches",
                  value: "5",
                  subtitle: "Active Branches",
                  icon: Icons.store,
                  color: AppColors.branches,
                ),
                DashboardStatCard(
                  title: "Pending PO",
                  value: "18",
                  subtitle: "Waiting Approval",
                  icon: Icons.pending_actions,
                  color: AppColors.pending,
                ),
              ],
            ),

            const SizedBox(height: 30),

            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  flex: 3,
                  child: AppCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          "Weekly Orders",
                          style: TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),

                        const SizedBox(height: 30),

                        SizedBox(
                          height: 250,
                          child: Row(
                            crossAxisAlignment: CrossAxisAlignment.end,
                            mainAxisAlignment: MainAxisAlignment.spaceAround,
                            children: const [
                              _ChartBar(day: "Mon", value: .45),
                              _ChartBar(day: "Tue", value: .65),
                              _ChartBar(day: "Wed", value: .80),
                              _ChartBar(day: "Thu", value: .55),
                              _ChartBar(day: "Fri", value: .95),
                              _ChartBar(day: "Sat", value: .75),
                              _ChartBar(day: "Sun", value: .60),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(width: 20),

                Expanded(
                  child: AppCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: const [
                        Text(
                          "Branches",
                          style: TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        SizedBox(height: 20),
                        _BranchTile("Smoke House"),
                        _BranchTile("Naiharn"),
                        _BranchTile("Kathu"),
                        _BranchTile("Bangtao"),
                        _BranchTile("Halal"),
                      ],
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 20),

            Row(
              children: [
                Expanded(
                  child: AppCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: const [
                        Text(
                          "Top Products",
                          style: TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        SizedBox(height: 20),
                        _ProductTile("Chicken Breast", "245 Orders"),
                        _ProductTile("French Fries", "201 Orders"),
                        _ProductTile("Beef Rib", "182 Orders"),
                        _ProductTile("Burger Bun", "151 Orders"),
                        _ProductTile("Cheddar Cheese", "143 Orders"),
                      ],
                    ),
                  ),
                ),

                const SizedBox(width: 20),

                Expanded(
                  child: AppCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: const [
                        Text(
                          "Latest Purchase Orders",
                          style: TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        SizedBox(height: 20),
                        _OrderTile("#PO-240701", "Smoke House"),
                        _OrderTile("#PO-240702", "Bangtao"),
                        _OrderTile("#PO-240703", "Kathu"),
                        _OrderTile("#PO-240704", "Halal"),
                        _OrderTile("#PO-240705", "Naiharn"),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _ChartBar extends StatelessWidget {
  const _ChartBar({required this.day, required this.value});

  final String day;
  final double value;

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        Container(
          width: 34,
          height: 180 * value,
          decoration: BoxDecoration(
            color: AppColors.orange,
            borderRadius: BorderRadius.circular(8),
          ),
        ),
        const SizedBox(height: 10),
        Text(day),
      ],
    );
  }
}

class _BranchTile extends StatelessWidget {
  const _BranchTile(this.name);

  final String name;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: const CircleAvatar(
        backgroundColor: AppColors.orange,
        child: Icon(Icons.store, color: Colors.white),
      ),
      title: Text(name),
      trailing: const Icon(Icons.chevron_right),
    );
  }
}

class _ProductTile extends StatelessWidget {
  const _ProductTile(this.name, this.qty);

  final String name;
  final String qty;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: const Icon(Icons.inventory_2, color: AppColors.orange),
      title: Text(name),
      subtitle: Text(qty),
    );
  }
}

class _OrderTile extends StatelessWidget {
  const _OrderTile(this.number, this.branch);

  final String number;
  final String branch;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: const CircleAvatar(
        backgroundColor: AppColors.orange,
        child: Icon(Icons.receipt_long, color: Colors.white),
      ),
      title: Text(number),
      subtitle: Text(branch),
      trailing: const Icon(Icons.arrow_forward_ios, size: 16),
    );
  }
}
