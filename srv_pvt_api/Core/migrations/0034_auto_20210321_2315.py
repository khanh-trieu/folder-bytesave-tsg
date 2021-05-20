# Generated by Django 2.1.15 on 2021-03-21 16:15

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Core', '0033_auto_20210319_1652'),
    ]

    operations = [
        migrations.AlterField(
            model_name='agents',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='bytesave_cycle',
            name='bytesave_expiration_date',
            field=models.IntegerField(default=1616343339, null=True),
        ),
        migrations.AlterField(
            model_name='bytesave_cycle',
            name='bytesave_start_date',
            field=models.IntegerField(default=1616343339, null=True),
        ),
        migrations.AlterField(
            model_name='bytesave_cycle',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='bytesave_cycle',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_check_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='customer_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='customer_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='log_contents',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='log_contents',
            name='time_log',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='log_contents',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='setting_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='setting_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_create_at',
            field=models.IntegerField(default=1616343339),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_update_at',
            field=models.IntegerField(default=1616343339),
        ),
    ]