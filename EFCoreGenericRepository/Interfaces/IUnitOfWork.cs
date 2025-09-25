using Microsoft.EntityFrameworkCore;

namespace EFCoreGenericRepository.Interfaces
{
    /// <summary>
    /// �Ƿ��͹�����Ԫ����ӿ�
    /// ����Э��һ��ҵ������ڶ�ͬһ DbContext �Ķ������д�룬
    /// �Ա㼯�п���������ʽ����ʽ�����ύʱ��������ɢ��� SaveChanges ���á�
    /// </summary>
    /// <remarks>
    /// ʹ��ָ����
    /// 1. ֻ��һ�μ�д��������ֱ���òִ������� UoW����
    /// 2. ��д������ԭ���ԣ����� <see cref="BeginTransactionAsync"/> -> (���д) -> <see cref="CommitAsync"/>��
    /// 3. ���ⳤ���񣺾���������Χ������С��׼�����ݷ��������⣩��
    /// 4. DbContext / UnitOfWork ��Ϊ Scoped������ Web ������ʹ�ã������̰߳�ȫ����Ҫ���̲߳���ʹ��ͬһʵ����
    /// </remarks>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// ��ʽ�������ݿ�����
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>�첽����</returns>
        /// <remarks>
        /// 1. ���л����ʱ�ٴε��ý������ԣ��ɸ�����Ҫ��Ϊ���쳣���ϸ�ģʽ����
        /// 2. �������ñ�����ֱ��ִ�� <see cref="CommitAsync"/>��
        ///    - EF Core ��һ�� <c>SaveChanges()</c> �ڲ���ʹ����ʽ���񣨱�֤������������Сԭ���ԣ���
        ///      ���޷����Ƕ�η�ɢ�� SaveChanges��
        /// 3. ���飺����ȷ��������д��������ɹ���ʧ�ܡ��ĳ��������ʽ���á�
        /// </remarks>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// �ύ��ǰ���������ʽ����ʱ��ִ��һ�α��档
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>�첽���񣨲�������Ӱ���������������չΪ Task&lt;int&gt;��</returns>
        /// <remarks>
        /// ��Ϊ��֧��
        /// �ѵ��� <see cref="BeginTransactionAsync"/>�������ڲ� <c>SaveChangesAsync()</c> ��ִ�����ݿ����� Commit��
        /// δ���� <see cref="BeginTransactionAsync"/>����ִ��һ�� <c>SaveChangesAsync()</c>��ʹ�� EF Core ��ʽ���񣩡�
        ///
        /// ע�⣺
        /// 1. ���� Commit ������ <c>SaveChangesAsync</c> ���쳣�������ϲ㲶������ <see cref="RollbackAsync"/>��ĳЩʵ�ֻ����ڲ��Զ� Rollback����
        /// 2. ����˳������������� Begin���������쳣����ǰĬ�Ͽ��ɣ����ɸ����Ŷӹ淶��Ϊ�ϸ�ģʽ��ֱ���׳�����
        /// 3. �ظ����� Commit���ڶ��������޻����ֻ���ٴδ���һ�� SaveChanges�����ܲ��������������壬���������
        /// </remarks>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// �ع���ǰ����������ڣ���
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>�첽����</returns>
        /// <remarks>
        /// 1. δ��ʼ����ʱ���ý������ԣ��޸����ã���
        /// 2. �ع���Ӱ�����ݿ����д�뵫δ�ύ�����ݣ��ڴ��и��ٵ�ʵ��״̬�Ա������ɰ����ֶ� Detach����
        /// 3. �����ڲ���ҵ�� / �����쳣ʱ���ã�ȷ�����ύ���ֳɹ��Ĳ�����
        /// </remarks>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// ֱ�ӵ��� DbContext.SaveChangesAsync�������������������ڣ���
        /// </summary>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>��Ӱ����������� EF Core ���أ�</returns>
        /// <remarks>
        /// ʹ�ó�����
        /// 1. ����Ҫ����д������ԭ���ԣ����ύ��ǰ���ۻ������
        /// 2. ����ʽ�����е��ã������Զ� Commit��������ʽ���� <see cref="CommitAsync"/>��
        ///
        /// ����
        /// - <see cref="SaveChangesAsync"/>��ֻ��һ�γ־û�������������
        /// - <see cref="CommitAsync"/>����ִ�� SaveChanges�����޴��󣩲��ύ����������ڣ���
        /// </remarks>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ���͹�����Ԫ����ӿ�
    /// �ض� DbContext �Ĺ�����Ԫ��չ�ӿ�
    /// </summary>
    /// <typeparam name="TDbContext">������ DbContext ����</typeparam>
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        /// <summary>
        /// �뵱ǰ������Ԫ����Ĳִ�ʵ����ʹ��ͬһ DbContext��
        /// д������δ��ʽ Commit ǰ�����Զ����� SaveChanges����������Ϊ autoSaveChanges:false��
        /// </summary>
        IRepository<TDbContext> Repository { get; }

        /// <summary>
        /// ֱ�ӷ��ʵײ� DbContext������ʹ�ã�����й©���ⲿ��νṹ��������
        /// </summary>
        TDbContext DbContext { get; }

        /// <summary>
        /// �Ƿ���ڻ����
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// ������ģʽ�İ�װ�������Զ� Begin / Commit / Rollback
        /// </summary>
        /// <param name="action">ʹ�òִ�ִ�е��첽����ί��</param>
        /// <param name="cancellationToken">ȡ������</param>
        /// <returns>�첽����</returns>
        /// <remarks>
        /// 1. ����ǰ�������Զ���������ִ��ί�У��ɹ��� Commit�������쳣�Զ� Rollback �������׳���
        /// 2. ����ǰ��������Ƕ�׵��ã��������ظ��������񣬽�������������
        /// 3. �ʺ���Ӧ�÷�����װ��һ��ҵ������ = һ�������߼�������ģ����롣
        /// </remarks>
        Task ExecuteInTransactionAsync(Func<IRepository<TDbContext>, Task> action, CancellationToken cancellationToken = default);
    }
}