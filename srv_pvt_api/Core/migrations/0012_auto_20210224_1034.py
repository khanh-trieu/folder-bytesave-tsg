# Generated by Django 2.1.4 on 2021-02-24 03:34

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Core', '0011_auto_20210224_1033'),
    ]

    operations = [
        migrations.AddField(
            model_name='loggin',
            name='name',
            field=models.CharField(max_length=250, null=True),
        ),
        migrations.AddField(
            model_name='loggin',
            name='pwd',
            field=models.CharField(max_length=250, null=True),
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='customer_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='customer_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_create_at',
            field=models.IntegerField(default=1614137660),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_update_at',
            field=models.IntegerField(default=1614137660),
        ),
    ]