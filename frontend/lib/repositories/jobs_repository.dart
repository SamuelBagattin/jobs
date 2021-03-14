import 'dart:convert';

import 'package:jobs/models/models.dart';
import 'package:http/http.dart' as http;

class JobsRepository{
  static const url = 'https://jobs.samuelbagattin.com/index.json';
  final client = http.Client();

  Future<JobsRootObject> getJobs() async {
    final response = await http.get(Uri.tryParse(url));
    return JobsRootObject.fromJson(jsonDecode(response.body));
  }
}
