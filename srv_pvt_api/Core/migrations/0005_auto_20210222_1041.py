# Generated by Django 2.1.15 on 2021-02-22 03:41

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Core', '0004_auto_20210219_0745'),
    ]

    operations = [
        migrations.AlterField(
            model_name='agents',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='service',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='versions',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_create_at',
            field=models.IntegerField(default=1613965304),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_update_at',
            field=models.IntegerField(default=1613965304),
        ),
    ]