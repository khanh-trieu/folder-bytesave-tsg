# Generated by Django 2.1.15 on 2021-03-08 02:55

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Core', '0018_auto_20210303_2356'),
    ]

    operations = [
        migrations.AddField(
            model_name='backup_bytesave',
            name='time_delete_file_in_LastVersion',
            field=models.IntegerField(default=30),
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='customer_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='customer_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_create_at',
            field=models.IntegerField(default=1615172112),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_update_at',
            field=models.IntegerField(default=1615172112),
        ),
    ]