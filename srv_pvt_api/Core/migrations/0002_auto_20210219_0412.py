# Generated by Django 2.1 on 2021-02-19 04:12

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Core', '0001_initial'),
    ]

    operations = [
        migrations.CreateModel(
            name='Customer_ByteSave',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('id_customer', models.IntegerField()),
                ('bytesave_email', models.CharField(blank=True, max_length=250, null=True)),
                ('bytesave_pwd', models.CharField(blank=True, max_length=250, null=True)),
                ('bytesave_amount_used', models.IntegerField(default=1)),
                ('bytesave_duration', models.IntegerField(blank=True, null=True)),
                ('bytesave_time_type', models.IntegerField(default=1)),
                ('bytesave_expiration_date', models.CharField(blank=True, max_length=250)),
            ],
        ),
        migrations.RemoveField(
            model_name='customers',
            name='bytesave_amount_used',
        ),
        migrations.RemoveField(
            model_name='customers',
            name='bytesave_duration',
        ),
        migrations.RemoveField(
            model_name='customers',
            name='bytesave_email',
        ),
        migrations.RemoveField(
            model_name='customers',
            name='bytesave_expiration_date',
        ),
        migrations.RemoveField(
            model_name='customers',
            name='bytesave_pwd',
        ),
        migrations.RemoveField(
            model_name='customers',
            name='bytesave_time_type',
        ),
        migrations.AlterField(
            model_name='agents',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='backup_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='connect_bytesave',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='customer_represent',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='customers',
            name='is_del',
            field=models.IntegerField(default=0),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='customers',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='loggin',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='metric_services',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='service',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_create_at',
            field=models.IntegerField(default=1613707917),
        ),
        migrations.AlterField(
            model_name='versions',
            name='time_update_at',
            field=models.IntegerField(default=1613707917),
        ),
    ]
