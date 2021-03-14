import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:jobs/models/models.dart';

class CompanyView extends StatelessWidget {
  final JobsRootObject jobs;

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
        child: Container(
            child: ExpansionPanelList(
      children: _getList(),
    )));
  }

  List<ExpansionPanel> _getList() {
    return this
        .jobs
        .companies
        .companies
        .map<Item>((e) => Item(associatedCompany: e))
        .map<ExpansionPanel>((e) => ExpansionPanel(
            isExpanded: e.isExpanded,
            headerBuilder: (context, isExpanded) => ListTile(
                  title: Text(e.associatedCompany.companyName),
                  subtitle:
                      Text(e.associatedCompany.mainTechnologies.join(" ")),
                ),
            body: Center(
                child: Column(
              children: e.associatedCompany.jobs
                  .map<Card>((job) => Card(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            ListTile(
                              title: Text(job.jobTitle),
                              subtitle: Text("Main Technos : " +
                                  job.mainTechnologies.join(" ") +
                                  "\nSecondary Technos : " +
                                  job.secondaryTechnologies.join(" ")),
                            )
                          ],
                        ),
                      ))
                  .toList(),
            ))))
        .toList();
  }

  CompanyView({this.jobs});
}

class Item {
  Item({this.isExpanded = false, @required this.associatedCompany});

  bool isExpanded = false;
  Company associatedCompany;
}
