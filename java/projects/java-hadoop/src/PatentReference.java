package org.action;

import java.io.IOException;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.conf.Configured;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.util.Tool;
import org.apache.hadoop.util.ToolRunner;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.Mapper;
import org.apache.hadoop.mapreduce.Reducer;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.input.KeyValueTextInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.mapreduce.lib.output.TextOutputFormat;

public class PatentReference extends Configured implements Tool {

  /**
   * The mapper implementation
   */
  public static class MapperInternal extends Mapper<Text, Text, Text, Text>{
   
    public void map(Text key, Text value, Context context)
        throws IOException, InterruptedException {
        context.write(value, key);
    }
  }
 
  /**
   * The reducer implementation
   */
  public static class ReducerInternal extends Reducer<Text, Text, Text, Text> {

    public void reduce(Text key, Iterable<Text> values, Context context)
        throws IOException, InterruptedException {

        StringBuilder sb = new StringBuilder();
        for (Text value : values) {
            if (sb.length() > 0) sb.append(",");
            sb.append(value.toString());
        }
        context.write(key, new Text(sb.toString()));
    }
  }

  /**
   * The tool runner
   */
  public int run(String[] args) throws Exception {
    Configuration conf = getConf();
    conf.set("mapreduce.input.keyvaluelinerecordreader.key.value.separator", ",");

    Job job = new Job(conf, "PatentReference");
    job.setJarByClass(PatentReference.class);

    job.setMapperClass(MapperInternal.class);
    job.setReducerClass(ReducerInternal.class);

    job.setInputFormatClass(KeyValueTextInputFormat.class);
    job.setOutputFormatClass(TextOutputFormat.class);

    job.setOutputKeyClass(Text.class);
    job.setOutputValueClass(Text.class);

    FileInputFormat.addInputPath(job, new Path(args[0]));
    FileOutputFormat.setOutputPath(job, new Path(args[1]));

    return job.waitForCompletion(true) ? 1 : 0;
  }

  public static void main(String[] args) throws Exception {
      int res = ToolRunner.run(new Configuration(), new PatentReference(), args);
      System.exit(res);
  }
}
